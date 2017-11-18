# TfsCodeReviewCloser
Tiny utility that searches for all TFS changeset code review requests, and closes them provided that all code review responses have been finished with “Looks Good”.

Reviews that were finished with “With Comments” and “Needs Work” will need to be closed manually.  I run it periodically through Windows Task Scheduler.  I appreciate it not because it saves me a lot of time but because it frequently spares me from the awful Team Explorer dialog.

The utility takes no action on shelveset reviews.  

# Command-line Parameters:
    -s, --server    (Default: http://tfs:8080/tfs) URL of Team Foundation Server
    -u, --user      Required. TFS user name (for finding your code review and setting "Closed By")

# Example Usage:
    TfsCodeReviewCloser.exe ---user "John Doe" --server http://tfs:8080/tfs

# Tips
* The project needs to download the CommandLineParser library with Nuget.
* If Microsoft.TeamFoundation references are not resolving,  DON'T PANIC.  Right-click the project, search for the problem assemblies, add them, and remove the original referneces.   Microsoft Visual Studio 12.0 TeamFoundation libraries are currently being referenced in the project.
* The utility should work fine with TFS 2010 and 2015, but might need tweaking if TFS workitems and/or workflows have been modified.

# License
Licensed under the MIT license. See LICENSE file in the project root for full license information.
