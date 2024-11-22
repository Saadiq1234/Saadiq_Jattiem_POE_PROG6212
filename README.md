INTRODUCTION For the final part of our POE of the semester, we are tasked with adding automation features to our lecturer view and our coordinator view, as well as introducing a new view called the HR view.

This document outlines the changes made based on lecturer feedback. However, since I received full marks for the last submission, I will explain all the changes that I deemed necessary for my application and describe the new features added.

For this part of the POE, there have been no changes to my database, and I am still using the WPF .NET FRAMEWORK functionality, with zero to minimal changes in design.

Changes For the POE, I have made the following changes:

The application under "Submit Claims" now includes the ability to save the hourly rate. The feature no longer has a fixed hourly rate; lecturers are allowed to input an hourly rate of their own. Part 3 Automation Features Lecturer View The lecturer view allows for the total amount of a claim to be automatically generated when a user inputs their Number of Sessions and Hourly Rate. This function is stored in the Submit Claim Window. Programme Coordinator View A feature has been added that automatically updates claims submitted to be approved if there are 5 or more sessions. Claims with less than 5 sessions will have their status changed to "pending," allowing programme coordinators to manually approve or reject them. HR VIEW The new HR view automatically approves user claims if they are set to "PENDING." Users can update lecturer information by searching for their name and surname. It also allows for the generation and downloading of reports based on approved claims. Git Hub Repo Link https:https://github.com/Saadiq1234/Saadiq_Jattiem_POE_PROG6212.git
Conclusion In conclusion, the final POE has successfully implemented essential automation features to enhance the usability of the application for lecturers and programme coordinators.

The lecturer view now allows for flexible input of hourly rates and automated calculation of claim totals, simplifying the claims process. The programme coordinator view incorporates an automated system for categorizing claims based on the number of sessions, streamlining the approval process. The new HR view automates the approval of claims and facilitates the downloading of reports, thus managing lecturer information more effectively. Overall, these changes significantly enhance user experience and administrative efficiency. The completed project is available on GitHub.

About
No description, website, or topics provided.
Resources
 Readme
 Activity
Stars
 0 stars
Watchers
 1 watching
Forks
 0 forks
Report repository
Releases
No releases published
Packages
No packages published
Languages
C#
100.0%
Footer
