# Quick Start Guide - Angular + .NET API POC

## Summary of What Was Created

This POC demonstrates a full-stack web application with:

### Backend (.NET 10)
- **Location:** `/backend`
- **API Controller:** `ProductsController.cs` - Provides REST endpoints at `/api/products`
- **Models:** `Product.cs` - Data model with Id, Name, Description, Price, Stock
- **Features:**
  - CORS enabled for `localhost:4200`
  - 5 mock products with realistic data
  - No database (in-memory mock data)

### Frontend (Angular 19)
- **Location:** `/frontend`
- **Service:** `ProductService` - Handles API communication
- **Component:** `ProductsComponent` - Displays products in a responsive grid
- **Features:**
  - Standalone component architecture
  - HTTP error handling
  - Loading states
  - Responsive design with CSS Grid

## How to Run the Application

### Terminal 1 - Start the Backend

```bash
cd /workspaces/azure-webapp-poc/backend
dotnet run
```

**Expected Output:**
```
...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7239
...
```

### Terminal 2 - Start the Frontend

```bash
cd /workspaces/azure-webapp-poc/frontend
npm start
```

**Expected Output:**
```
...
✔ Server online at http://localhost:4200  
```

### Open in Browser

Navigate to: `http://localhost:4200`

You should see a "Product Store" page with 5 products displayed in a grid.

## What the Application Does

1. **On Page Load:**
   - Angular component initializes
   - ProductService is injected
   - `ngOnInit()` calls `loadProducts()`

2. **Loading Products:**
   - Service makes HTTP GET to `https://localhost:7239/api/products`
   - Backend returns mock product data as JSON
   - Component displays products in responsive grid

3. **Display:**
   - Product cards show: Name, Description, Price, Stock
   - Products with low stock (<20 units) highlighted in red
   - Clean, modern styling with hover effects

## API Endpoints

### GET /api/products
Returns all products
```json
[
  {
    "id": 1,
    "name": "Laptop",
    "description": "High-performance laptop for developers",
    "price": 999.99,
    "stock": 15
  },
  ...
]
```

### GET /api/products/{id}
Returns a single product by ID
```json
{
  "id": 1,
  "name": "Laptop",
  "description": "High-performance laptop for developers",
  "price": 999.99,
  "stock": 15
}
```

## Testing the API

You can test the API directly using curl:

```bash
# Get all products
curl https://localhost:7239/api/products -k

# Get specific product (using -k to ignore certificate warning)
curl https://localhost:7239/api/products/1 -k
```

Or use VS Code REST Client:
```
GET https://localhost:7239/api/products HTTP/1.1
Content-Type: application/json
```

## Architecture Diagram

```
┌─────────────────────────────────────────────────────┐
│         Browser (localhost:4200)                    │
│  ┌──────────────────────────────────────────────┐   │
│  │  Angular Application (frontend/)             │   │
│  │  ┌────────────────────────────────────────┐  │   │
│  │  │ ProductsComponent                      │  │   │
│  │  └────┬─────────────────────────────────┬─┘  │   │
│  │       │ ngOnInit()                      │    │   │
│  │       └────────────────────────────────┘    │   │
│  │  ┌───────────────────────────────────────┐  │   │
│  │  │ ProductService                        │  │   │
│  │  │ - getProducts()                       │  │   │
│  │  │ - HTTP GET request                    │  │   │
│  │  └────────┬────────────────────────────┬─┘  │   │
│  │           │                            │    │   │
│  └───────────┼────────────────────────────┼────┘   │
│              │                            │        │
│   HTTP GET   │                            │        │
│   /api/products                           │        │
│              ▼                            │        │
│  ┌─────────────────────────────────────────┐    │
│  │ HTTPS://localhost:7239                  │    │
│  │                                         │    │
│  │ .NET 10 Web API (backend/)              │    │
│  │ ┌─────────────────────────────────────┐ │    │
│  │ │ ProductsController                  │ │    │
│  │ │ - GET /api/products                 │ │    │
│  │ │ - GET /api/products/{id}            │ │    │
│  │ └────────────┬──────────────────────┬─┘ │    │
│  │              │                      │   │    │
│  │               ──────────────────────┘   │    │
│  │ ┌─────────────────────────────────────┐ │    │
│  │ │ Mock Data (In-Memory)               │ │    │
│  │ │ - List<Product>                     │ │    │
│  │ │   - 5 sample products               │ │    │
│  │ │   - No database needed              │ │    │
│  │ └─────────────────────────────────────┘ │    │
│  └─────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────┘
```

## Directory Structure

```
azure-webapp-poc/
├── README.md                               # Main project documentation
├── QUICKSTART.md                          # This file
│
├── backend/                                # .NET API
│   ├── Controllers/
│   │   └── ProductsController.cs         # REST API endpoints
│   ├── Models/
│   │   └── Product.cs                    # Data model
│   ├── Properties/
│   │   └── launchSettings.json           # IIS Express config
│   ├── bin/                              # Build output
│   ├── obj/                              # Build artifacts
│   ├── Program.cs                        # Application bootstrap
│   ├── backend.csproj                    # Project configuration
│   ├── appsettings.json                  # App config
│   └── appsettings.Development.json      # Dev config
│
└── frontend/                               # Angular App
    ├── src/
    │   ├── app/
    │   │   ├── components/
    │   │   │   └── products/
    │   │   │       ├── products.component.ts    # Component logic
    │   │   │       ├── products.component.html  # Template
    │   │   │       └── products.component.css   # Styles
    │   │   ├── services/
    │   │   │   └── product.service.ts    # API service
    │   │   ├── app.ts                    # Root component
    │   │   ├── app.html                  # Root template
    │   │   ├── app.routes.ts             # Routing config
    │   │   └── app.config.ts             # App config
    │   ├── main.ts                       # Bootstrap
    │   ├── index.html                    # Entry HTML
    │   └── styles.css                    # Global styles
    ├── public/
    │   └── favicon.ico
    ├── dist/                             # Build output
    ├── node_modules/                     # Dependencies
    ├── angular.json                      # Angular config
    ├── package.json                      # npm dependencies
    ├── tsconfig.json                     # TypeScript config
    └── README.md                         # Frontend docs
```

## Debugging Tips

### Backend Issues
- Check if port 7239 is already in use
- Verify .NET 10 SDK: `dotnet --version`
- Check CORS configuration in `Program.cs`
- Use browser DevTools Network tab to see API calls

### Frontend Issues  
- Check browser console (F12) for errors
- Verify API URL in `ProductService`
- Check that backend is running before starting frontend
- Clear browser cache if experiencing issues

### HTTPS Certificate Issues
If you get certificate warnings:
```bash
dotnet dev-certs https --trust
```

## Next Steps

1. **Test the API:** Use curl or Postman to test endpoints
2. **Modify Mock Data:** Edit `ProductsController.cs` to change products
3. **Add Features:** 
   - Product detail view
   - Shopping cart
   - Search/filter
   - Pagination
4. **Connect to Database:** Replace mock data with real database
5. **Deploy:** Use Azure App Service for deployment

## Useful Commands

```bash
# Backend
cd backend
dotnet run                          # Run development server
dotnet build                        # Build project
dotnet clean                        # Clean build artifacts
dotnet dev-certs https --trust      # Trust dev certificate

# Frontend
cd frontend
npm install                         # Install dependencies
npm start                           # Run dev server on :4200
npm run build                       # Production build
npm run build:prerender             # Build with SSR
```

## Common Ports

- **Backend:** https://localhost:7239
- **Frontend:** http://localhost:4200
- **Backend HTTP:** http://localhost:5283 (if HTTPS redirect disabled)

## Troubleshooting Checklist

- [ ] .NET 10 SDK installed: `dotnet --version`
- [ ] Node.js v24+ installed: `node --version`
- [ ] Backend running on port 7239
- [ ] Frontend running on port 4200
- [ ] CORS enabled in backend Program.cs
- [ ] API URL correct in ProductService
- [ ] Browser network tab shows successful API call
- [ ] No browser console errors

---

**Happy coding!** If you have questions, refer to the main [README.md](README.md) file.
