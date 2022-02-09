# SocialFood
## What it is?
This project was born with the aim of creating a social network between friends to share photos of the dishes we eat while we are out.  
The project was developed in the summer between the end of high school and the beginning of university, relying mainly on PHP. I wanted to rewrite all the code using mainly **C#** and **ASP.NET Core**, after using it in the third year internship.

## Key points
- The data is saved on a MySQL database
- The images are saved on Azure storage account, locally they are saved on disk
- The project uses JsonWebToken-based authentication
- The server notifies the PWA of the following events:
    - New image uploaded by a friend
    - Someone added you to friends
- Continuous Delivery is used on Github Actions

## How?
The project is divided into **5 sub-projects**:

### SocialFood.API
It manages authentication, uploading photos, sending notifications and managing all the actions that the user does within the app

### SocialFood.Web
It is a Blazor PWA, which communicates with the server. At the time of login it saves the JWT on the browser memory. After authenticating, it asks the user if they want to receive notifications for events from the server.

### SocialFood.Data
It is the layer that deals with interacting with the database

### SocialFood.StorageProviders
It is the layer that takes care of saving the files on the disk if you work locally, or on the Azure storage account if the app is in production

### SocialFood.Shared
It is a common class library between the API server and the PWA
