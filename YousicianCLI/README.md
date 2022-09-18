# Yousician Command Line Interface

## Scenario

The Yousician CLI is a product early in its development. While it currently
only contains signup functionality, there are multiple developers working on
features of the app. Your task is to implement a new feature.

A number of features will be A/B tested, meaning that the backend assigns users
to different groups, which then receive different functionality in the app.
These experiments may run for a while, but are eventually concluded, at which
point the winning version is selected to be the only version, and support for
the alternate version is removed from the app. 

## Process

You should follow these steps when implementing the changes:
1. Create a new branch for your feature.
2. Implement the changes as you would, if you were creating a pull request.
3. Once you are finished, create a commit which adds two markdown files:
   * `MESSAGE.md`, containing a message you would use, if you were to open a
     pull request. Be honest about how you feel about the changes you made.
   * `NOTES.md`, containing short notes about the following subjects:
     * What aspects were the most challenging, and what new did you learn.
     * Approximately how much time did you spend on implementation and learning.
4. Use `git bundle create` to create a bundle containing your branch to send us
   as the deliverable for this task.

## Task

Your task is to extend the current signup flow to support an A/B test:
* In the very beginning of the signup flow, the application should call the
  `IBackend.GetExperimentGroup` method to determine the experiment group.
  * A loading indicator with the message `Messages.Loading` should be displayed
    while this operation is in progress.
  * If this operation is to fail, an exception should be propagated to be
    handled by the `Program` class.
* If the experiment group is A, the logic from here on should be the same as it
  is currently.
* If the experiment group is B, the application should implement the following
  logic:
  * First prompt the user for the instrument, but do not call any backend
    methods at this point.
  * Next, prompt the user for their credentials, and register the user using the
    `IBackend.RegisterUser`, including the optional `instrument` parameter. 
  * If registering the user fails for any reason, both username and password
    should be prompted for again, and signing up should happen with the 
    instrument selected at the beginning of the flow.

## Limitations

Do **not** make any changes to the following code:
* The `Program` class
  * This also means that the `SignupFlow` constructor and `Run` method signatures
    or return types can not be modified.
* The `IBackend` or `IUserInterface` interfaces

## About the Backend

The project includes a mock implementation of the `IBackend` interface, which is
used by default when running the project. This is a very unreliable backend,
which will randomly fail. It's also worth mentioning that the "username already
in use" logic is completely random, and has nothing to do with the given input.

The implementation includes a bit of configurability, but feel free to extend it
further. Or even write a new mock implementation, if you find that useful.

## Dependencies & Resources

You may use your favorite C# IDE to edit the code. If you don't have one,
[Rider](https://www.jetbrains.com/rider/) is a good option, and has a 30 day
free trial.

If you prefer text editors over IDEs, you might want to know that we have
a strict policy about using Rider at Yousician. But if you want to go with a
text editor now, you may use the `dotnet` command line to interface with the
project.

The project has the following dependencies, which should get auto-installed by
NuGet:
* `System.Reactive`: Reactive Extensions for building asynchronous code. If you
  are not familiar with Rx, check out the following resources:
  * [reactivex.io](http://reactivex.io/): Useful reference documentation for Rx
  * [introtorx.com](http://introtorx.com/): A more in-depth introduction, if you
    want to get more familiar with Rx.
* `NUnit`: Test framework, documentation available
  [online](https://docs.nunit.org/)
* `NSubstitute`: Mocking library, documentation avilable
  [online](https://nsubstitute.github.io/help.html)
