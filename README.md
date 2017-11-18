# TfsCodeReviewCloser
Tiny utility that searches for all TFS changeset code review requests, and closes them provided that all code review responses have been finished with “Looks Good”.

Reviews that were finished with “With Comments” and “Needs Work” will need to be closed manually.  I run it periodically through Windows Task Scheduler.  I appreciate it not because it saves me a lot of time but because it frequently spares me from the awful Team Explorer dialog.

The utility takes no action on shelveset reviews.

# Command-line Parameters:
    -s, --server    (Default: http://tfs:8080/tfs) URL of Team Foundation Server
    -u, --user      Required. TFS user name (for finding your code review and setting "Closed By")

# Example Usage:
    TfsCodeReviewCloser.exe ---user "John Doe" --server http://tfs:8080/tfs

# License
Licensed under the MIT license. See LICENSE file in the project root for full license information.
