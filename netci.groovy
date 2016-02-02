// Import the utility functionality.
import jobs.generation.Utilities;

// Defines a the new of the repo, used elsewhere in the file
def project = GithubProject

// Generate the builds for debug and release, commit and PRJob
[true, false].each { isPR -> // Defines a closure over true and false, value assigned to isPR
    ['Debug', 'Release'].each { configuration ->
        
        // Determine the name for the new job.  The first parameter is the project,
        // the second parameter is the base name for the job, and the last parameter
        // is a boolean indicating whether the job will be a PR job.  If true, the
        // suffix _prtest will be appended.
        def newJobName = Utilities.getFullJobName(project, configuration, isPR)
        
        // Define build string
        def buildString = ".\\build.ps1 ${configuration}"

        // Create a new job with the specified name.  The brace opens a new closure
        // and calls made within that closure apply to the newly created job.
        def newJob = job(newJobName) {
            label('windows')
            
            // This opens the set of build steps that will be run.
            steps {
                powerShell(buildString)
            }
        }
        
        // This call performs remaining common job setup on the newly created job.
        // This is used most commonly for simple inner loop testing.
        // It does the following:
        //   1. Sets up source control for the project.
        //   2. Adds a push trigger if the job is a PR job
        //   3. Adds a github PR trigger if the job is a PR job.
        //      The optional context (label that you see on github in the PR checks) is added.
        //      If not provided the context defaults to the job name.
        //   4. Adds standard options for build retention and timeouts
        //   5. Adds standard parameters for PR and push jobs.
        //      These allow PR jobs to be used for simple private testing, for instance.
        // See the documentation for this function to see additional optional parameters.
        Utilities.simpleInnerLoopJobSetup(newJob, project, isPR, "Windows ${configuration}")

		//Upload test results
		Utilities.addXUnitDotNETResults(newJob, '**/testresults.xml')

    }
}

