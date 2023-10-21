# .NET Core 6 RESTful API for Managing User Notes

## Introduction

This RESTful API, built using .NET Core 6, allows users to manage their notes. It provides Create, Read, Update, and Delete (CRUD) operations for notes, and it enforces authentication to protect sensitive actions. This README provides instructions on how to run and test the API.

## Prerequisites

Before running the project, ensure you have the following prerequisites:

- .NET 6 SDK
- Visual Studio 2022
- Git

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/yourusername/your-repo.git
Navigate to the Project Directory

bash
Copy code
cd notesApp.API
Build the Project

bash
Copy code
dotnet build
Run the API

bash
Copy code
dotnet run
The API should now be accessible at http://localhost:5000 or a different URL based on your configuration.

API Endpoints
Users
User Registration
Method: POST
URL: https://localhost:5000/api/user/register
No authentication required.
Request Body:
json
Copy code
{
"email": "yourusername@das.com",
"password": "Password@123",
"userName": "your-username" // Optional
}
Response:
json
Copy code
{
"succeeded": true,
"errors": []
}

User Login

Method: POST
URL: https://localhost:5000/api/user/login
No authentication required.
Request Body:
json
Copy code
{
"email": "your-email@example.com",
"password": "your-password"
}
Response:
json
Copy code
{
"token": "your-authentication-token"
}

Notes
all of the following API's needs a Bearer Token
Create a Note

Method: POST
https://localhost:7002/api/Notes/AddNote
Requires authentication with a JWT token.
Request Body:
json
Copy code
{
//both are optional but one of them is required
"title": "My Note",
"description": "This is a sample note."
}
Response:
json
Copy code
{
"id": "241408c7-95a7-4dbd-90e7-ad9b6623b6f1",
"title": "My Note",
"description": "This is a sample note.",
"dateCreated": "2023-10-21T17:46:01.0840649+03:00",
"userId": "d126868d-a951-4e34-9cbf-a833ca9acf40",
"isDeleted": false,
"deletedAt": null
}

Get All Notes

Method: GET
URL: https://localhost:7002/api/Notes/GetAllNotes
Requires authentication with a JWT token.
Response:
json
Copy code
[
{
"id": "241408c7-95a7-4dbd-90e7-ad9b6623b6f1",
"title": "My Note",
"description": "This is a sample note.",
"dateCreated": "2023-10-21T17:46:01.0840649+03:00",
"userId": "d126868d-a951-4e34-9cbf-a833ca9acf40",
"isDeleted": false,
"deletedAt": null
}
// Add more notes here...
]
Get a Note by ID

Method: GET
URL: https://localhost:7002/api/Notes/GetNoteById/{id}
Requires authentication with a JWT token.
Response:
json
Copy code
{
"id": "241408c7-95a7-4dbd-90e7-ad9b6623b6f1",
"title": "My Note",
"description": "This is a sample note.",
"dateCreated": "2023-10-21T17:46:01.0840649+03:00",
"userId": "d126868d-a951-4e34-9cbf-a833ca9acf40",
"isDeleted": false,
"deletedAt": null
}
Update a Note by ID

Method: PUT
URL: https://localhost:7002/api/Notes/UpdateNoteById/{id}
Requires authentication with a JWT token.
Request Body:
json
Copy code
{
"title": "Updated Note",
"description": "This is an updated note."
}
Response:
json
Copy code
{
"id": "241408c7-95a7-4dbd-90e7-ad9b6623b6f1",
"title": "Updated Note",
"description": "This is an updated note.",
"dateCreated": "2023-10-21T17:46:01.0840649+03:00",
"userId": "d126868d-a951-4e34-9cbf-a833ca9acf40",
"isDeleted": false,
"deletedAt": null
}
Delete a Note by ID

Method: DELETE
URL: https://localhost:7002/api/Notes/DeleteNoteById/{id}
Requires authentication with a JWT token.
No request body is needed.
Response: Success message.

Authentication
This API uses token-based authentication. To access protected endpoints, include a valid JWT token in the 'Authorization' header as a bearer token. For example:

Copy code
Authorization: Bearer your-authentication-token

Data Storage
The API uses an in-memory database for simplicity. In a production environment, you should consider using a more robust database system.

Testing the API
You can test the API using tools like Postman or Swagger. Here are some sample API requests:

Register a user: POST http://localhost:5000/api/user/register with the user's email and password.

Log in to obtain an authentication token: POST http://localhost:5000/api/user/login with the user's email and password. You'll receive an authentication token in the response.

Use the token to access protected endpoints, such as creating, updating, and deleting notes.

Important Notes
Ensure that you keep your authentication token secure.
All requests to protected endpoints should include the JWT token in the 'Authorization' header.
Sample Code
You can find the sample code for the API controllers in the NotesController.cs and UserController.cs files. These controllers provide the CRUD operations for notes and user registration/authentication.

Error Handling
The API handles various error cases and provides appropriate HTTP status codes and error messages.

Additional Notes
This README serves as a quick reference for running and testing the API. Please refer to the API controllers for detailed information about endpoints and their behavior.

For any questions or issues, please contact Your Name.

Happy coding!
