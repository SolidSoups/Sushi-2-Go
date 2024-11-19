# Sushi-2-Go
![poster](/readme_images/poster.png)

## Information
### Description
Sushi 2 Go is a endless runner where you play as a shrimp sushi running on a conveyor belt trying to avoid obstacles and stay alive.
### History
During our first year at Future Games Malmö, we were tasked with creating an endless runner for Game Project 1. The project would take 3 weeks and we felt very satisfied with the finished project. And so we decided to continue working on it on our own time.

## Setting up and installation (Windows)
Please begin by creating a github account and messaging *Elias* to be added to the project as a collaborator.

To install this project on your own computer, you will need these dependencies (make sure to install them in the following order):
1. **[Unity 6 (6000.0.24f1)](https://unity.com/releases/editor/whats-new/6000.0.24)**
2. **[Github Desktop](https://desktop.github.com/download/)**
    1. After installing Github Desktop, please log in with your Github account.
3. **[Git](https://git-scm.com/downloads)**
4. **Git LFS (Large File Storage)**
   1. Make sure **Github Desktop** and **Git** are installed.
   2. Open up the command prompt by searching for "cmd" in the *start menu*.
   3. Verify **Git** installation by writing the prompt `git` and pressing enter
      ![command prompt git verification](/readme_images/gitverification.png)
   4. Write `git lfs install` into the command prompt and press enter
      ![command prompt git lfs install](/readme_images/gitlfsinstall.png)

Then you can set up the workspace on the computer.
1. Start by opening up **Github Desktop**
2. Click on *Clone an existing repository*
3. Select `Sushi-2-Go` and clone it.
4. Once it's finished, open up **Unity Hub** and in Projects click on *Add project from disk*
5. Find the location where the `Sushi-2-Go` repository was installed and select the root folder.
6. You should now be able to open up the project on your computer! :D

## Rules and workflow
### Branches
Github uses branches to create copies of the repository. This is useful when adding new features, game mechanics vice versa. You should definitely read up on branching [here](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/about-branches). 

One of the most important branches in the repository is the `main` or `master` branch. **Work should never be done on this branch**. Instead of working directly, changes should be merged via *pull requests* from other feature branches.