# GitHub Actions Manual Approval Pipeline

Showcase for a manual approval pipeline with unit testing. This example is using NextJS T3 Boilerplate application for representation purposes.

Once `push` or `pull-request` happen on `manual-approval` branch build artifact will be created and stored upon completing the build step the artifact will be preserved for a set amount of time (90 days). Upon reaching the end of each stage/job, a github issue will be created that will require approval in form of a comment in order for the pipeline to proceed. 