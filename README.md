# Checkout API

## Overview
This Checkout API allows users to manage products and process checkouts securely using an API key for authentication. The API supports user creation, product management, checkout initiation, modification, and completion.

---
## **Setup & Installation**

### **1. Clone the Repository**
```sh
git clone https://github.com/alphmalesela/checkout-api.git
cd checkout-api
```

### **2. Install Dependencies**
Ensure you have **.NET 6+** installed. Then, run:
```sh
dotnet restore
```

### **3. Configure Database**
This API uses **SQLite** with Entity Framework Core.

Run migrations:
```sh
dotnet ef database update
```

### **4. Run the API**
```sh
dotnet run
```

The API should now be available at:
```
http://localhost:5016
```

---
## **Authentication: API Key Usage**
This API uses **API Key authentication**.

### **Step 1: Create a User & Get API Key**
#### **Endpoint**: `POST /api/users`

##### **Request**:
```json
{
    "username": "john_doe"
}
```

##### **Response**:
```json
{
    "apiKey": "abc123xyz"
}
```

### **Step 2: Use API Key in Requests**
For **protected endpoints**, include the API key in the `Authorization` header:
```
Authorization: Bearer YOUR_API_KEY
```

#### **Example (GET /api/products)**:
```sh
curl -H "Authorization: Bearer YOUR_API_KEY" -X GET http://localhost:5016/api/products
```

---
## **Swagger Integration**
Swagger is available at:
```
http://localhost:5016/swagger/index.html
```

### **Using API Key in Swagger**
1. Open Swagger UI.
2. Click **"Authorize"**.
3. Enter your API Key as:
   ```
   Bearer YOUR_API_KEY
   ```
4. Click **"Authorize"** and make authenticated requests.

---
## **API Endpoints**

### **User Management**
| Method | Endpoint | Description |
|--------|---------|-------------|
| `POST` | `/api/users` | Creates a user and returns an API key |

### **Product Management**
| Method | Endpoint | Description |
|--------|---------|-------------|
| `POST` | `/api/products` | Adds a new product |
| `PUT` | `/api/products/{productId}` | Updates a product |
| `DELETE` | `/api/products/{productId}` | Deletes a product |
| `GET` | `/api/products` | Lists all products |
| `GET` | `/api/products/others` | Lists products owned by other users |

### **Checkout Process**
| Method | Endpoint | Description |
|--------|---------|-------------|
| `POST` | `/api/checkout` | Starts a checkout session |
| `PUT` | `/api/checkout/{checkoutId}` | Modifies an existing checkout |
| `DELETE` | `/api/checkout/{checkoutId}` | Cancels an active checkout |
| `POST` | `/api/checkout/{checkoutId}/complete` | Finalizes the checkout |
| `GET` | `/api/checkout/{checkoutId}` | Retrieves checkout details |

---
## **License**
This project is licensed under the MIT License.

