
# Hotel management API

The HotelAPI project is a RESTful API built with .NET 6, designed to streamline hotel operations by providing role-based access for different user types.
Admins and managers can manage hotel operations such as room availability and guest records, while guests can browse hotels, check availability, and make reservations.

---

## Features

### **User Authentication**  
Secure access is ensured using **JWT (JSON Web Tokens)**, allowing users to authenticate and receive a token for subsequent requests.

### **Role-Based Authorization**  
Users are granted access to specific resources and operations based on their assigned roles (e.g., Admin, Manager, Guest).

### **CRUD Operations**  
Supports the full range of operations for managing restaurant data, such as menus, orders, and reservations.

### **Static File Handling**  
The API can manage static files like images, PDFs, etc.

### **Pagination**  
The API supports pagination for data-heavy endpoints, providing:
- **Total Pages**: The number of pages available (configurable).
- **Results Per Page**: The number of items displayed per page (configurable).
- **Total Items**: The total count of elements in the database.
- **Sorting**: Data can be sorted by various attributes like name, date, etc.
- **Filtering**: Results can be narrowed down by applying specific filters (e.g., by cuisine type, rating, location).

### **Automatic Database Seeder**  
The application includes an automatic seeder that populates the database with sample data when the application starts, providing a ready-to-use environment for testing.
### **PDF report generation**
The hotel owner can generate three types of reports: financial reports, occupancy reports, and guest reports. Each report contains important statistics relevant to hotel operations.
### **Email notifications**
When a user makes a new reservation, a confirmation email is sent to the email address associated with their registered account.
---

## Architecture & Design

### **Dependency Injection (DI)**  
Used extensively to manage dependencies, promoting modularity, maintainability, and testability.

### **DTO Mapping**  
Data Transfer Objects (DTOs) are used for efficient data representation between layers.

### **Fluent Validation**  
Ensures validation rules are applied consistently and declaratively across requests.

---

## Technologies

The following technologies are used in the project:

- **ASP.NET Core 6**: For building the API.
- **Entity Framework**: ORM for interacting with the database.
- **LINQ**: For querying data.
- **DTO Mapping**: Simplifies data transfer between API layers.
- **Fluent Validation**: For input validation.
- **JWT Authentication**: Secures endpoints with JSON Web Tokens.
- **Swagger**: For interactive API documentation and testing.
- **Bogus**: Used to generate fake data for testing purposes.
- **MailKit**: For sending and receiving emails via SMTP.
- **QuestPDF**: Used for generating high-quality PDF reports.
---

## Integration Tests

The project includes **integration tests** implemented using **xUnit** and **Moq** to ensure the reliability of API endpoints and interactions.

### **Test Frameworks**
- **xUnit**: A robust and flexible testing framework for .NET applications.
- **Moq**: A popular mocking library for .NET that allows creating and configuring mock objects to isolate tests.

### **Key Scenarios Tested**
- **Authentication and Authorization**: Verifying that only authenticated users with proper roles can access secured endpoints.
- **CRUD Operations**: Ensuring data creation, retrieval, updates, and deletion functions work as expected.
- **Validation**: Testing that invalid inputs are properly handled and validated.
- **Database Interactions**: Mocking database interactions to test business logic without relying on the actual database.

