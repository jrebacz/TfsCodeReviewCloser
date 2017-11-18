// Copyright (c) Jeff Rebacz
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace TfsCodeReviewCloser
{
    class CodeReviewCloser
    {
        private string userName;
        private Microsoft.TeamFoundation.Client.TfsTeamProjectCollection teamProjectCollection;
        private VersionControlServer versionControl;
        private WorkItemStore workItemStore;

        public CodeReviewCloser(string tfsServer, string tfsUserName)
        {
            userName = tfsUserName;
            teamProjectCollection = new TfsTeamProjectCollection(new Uri(tfsServer));

            workItemStore = teamProjectCollection.GetService<WorkItemStore>();
            versionControl = teamProjectCollection.GetService<VersionControlServer>();
        }

        public void CloseCompleted()
        {
            // Run a query, get all outstanding changeset code review requests for user
            WorkItemCollection queryResults = workItemStore.Query(
               "Select * " +
               "From WorkItems " +
               "Where [Work Item Type] = 'Code Review Request' " +
               "And [Created by] = '" + userName + "'" +
               "And [State] = 'Requested'" +
               "And [Associated Context Type] = 'Changeset'"
               );

            Console.WriteLine("Found {0} code review requests by {1}", queryResults.Count, userName);
            foreach (WorkItem codeReviewRequest in queryResults)
            {
                if (IsReadyToClose(codeReviewRequest))
                {
                    Console.WriteLine("Closing " + codeReviewRequest.Id + ": " + codeReviewRequest.Title);

                    codeReviewRequest.Fields["State Code"].Value = 1;
                    codeReviewRequest.Fields["Closed Status"].Value = "Checked-in";
                    codeReviewRequest.Fields["Closed Status Code"].Value = 3;
                    codeReviewRequest.Fields["Closed By"].Value = userName;
                    codeReviewRequest.Fields["Assigned To"].Value = "";
                    codeReviewRequest.Reason = "Closed";
                    codeReviewRequest.State = "Closed";

                    if (codeReviewRequest.IsValid())
                    {
                        try
                        {
                            codeReviewRequest.Save();
                        }
                        catch (ValidationException exception)
                        {
                            Console.Error.WriteLine("Error attempting to close code review (id={0}).", codeReviewRequest.Id);
                            Console.Error.WriteLine(exception.Message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Not ready to close " + codeReviewRequest.Id + ": " + codeReviewRequest.Title);
                }
            }
        }

        private bool IsReadyToClose(WorkItem codeReviewRequest)
        {
            bool looks_good = false; // Must have 1 or more "looks good" responses to close a review.

            foreach (WorkItemLink child_link in codeReviewRequest.WorkItemLinks)
            {
                if (!child_link.LinkTypeEnd.Name.Equals("Child"))
                {
                    continue;
                }

                WorkItem child = workItemStore.GetWorkItem(child_link.TargetId);

                if (child.Fields["Work Item Type"].Value.Equals("Code Review Response"))
                {
                    if (
                        child.State.Equals("Closed")
                        &&
                        child.Fields["Closed Status"].Value.Equals("Looks Good"))
                    {
                        looks_good = true;
                        break;
                    }
                    else if (
                        child.State.Equals("Closed")
                        &&
                        child.Fields["Closed Status"].Value.Equals("Removed"))
                    {
                        continue;
                    }
                    else
                    {
                        return false;   // outstanding response, review is ineligible for closing.
                    }
                }
            }

            return looks_good;
        }
    }
}
