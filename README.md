# Sushi-2-Go
![poster](/readme_images/poster.png)

## Information
### Description
Tuna, Salmon, Squid... your three dearest friends, gone—devoured by the merciless machine of hunger. Now, it’s just you. A lone shrimp sushi on a conveyor belt of doom. Run... RUN FAST. Escape this endless nightmare, defy your fate, and avenge the fallen. Break the cycle, dodge every obstacle, and fight to survive. Don’t let them eat you. This is your story. This is Sushi 2 Go.
### History
In our first year at Future Games Malmö, we faced a challenge: create an endless runner for Game Project 1 in just three weeks. We poured ourselves into it, every detail reflecting our determination.

When the deadline arrived, we were proud but not satisfied. The project felt unfinished, brimming with potential. We couldn’t walk away. Despite school and limited time, we kept working on it, turning an assignment into a true passion.

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
### Commits
Please make sure to keep your commits short. When you make changes on your own branch, and you add for example *new assets*, *changes to scene* or *rearranging the hierarchy*, commit those first and then 
### Branches
Github uses branches to create copies of the repository. This is useful when adding new features, game mechanics vice versa. You should definitely read up on branching [here](https://docs.github.com/en/pull-requests/collaborating-with-pull-requests/proposing-changes-to-your-work-with-pull-requests/about-branches). 

One of the most important branches in the repository is the `main` or `master` branch. **Work should never be done on this branch**. Instead of working directly, changes should be merged via *pull requests* from other feature branches.