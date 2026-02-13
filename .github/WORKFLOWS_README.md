# GitHub Actions CI/CD Workflows

This project includes two different CI/CD workflows for deploying to Azure App Service. Choose the one that best fits your deployment strategy.

## Workflow Options

### Option 1: Combined Deployment (Recommended) 📦
**File:** `.github/workflows/deploy.yml`

**Architecture:**
- Single Azure App Service
- Backend .NET API serves both API endpoints and frontend static files
- Frontend built and copied to `wwwroot/`
- Simpler setup with lower costs

**Best For:**
- Small to medium projects
- Cost-conscious deployments
- Simple architecture
- Easier management

**Pros:**
✅ Single app service (lower cost)
✅ No CORS issues between frontend/backend
✅ Simpler CI/CD pipeline
✅ Easier to manage and scale

**Cons:**
❌ Backend performance affected by frontend static file serving
❌ Harder to scale frontend independently
❌ Both must be deployed together

### Option 2: Separate Deployment 🔀
**File:** `.github/workflows/deploy-separate.yml`

**Architecture:**
- Two Azure App Services
- Backend .NET API only serves API endpoints
- Frontend served from separate service/CDN
- Parallel independent builds

**Best For:**
- Large scale applications
- High traffic frontends
- Independent scaling needs
- Team structure matches separation

**Pros:**
✅ Independent scaling
✅ Can deploy independently
✅ Better for CI/CD parallelization
✅ Better separation of concerns
✅ Can use CDN for frontend

**Cons:**
❌ Higher infrastructure costs
❌ CORS must be properly configured
❌ More complex setup
❌ More services to manage

## Setup Steps

### Prerequisites
- Azure subscription
- GitHub repository
- Azure CLI or Azure Portal access

### 1. Choose Your Deployment Strategy

**For Combined Deployment (Option 1):**
```bash
# You only need one App Service for both backend and frontend
# Skip to "Create Backend App Service" section
```

**For Separate Deployment (Option 2):**
```bash
# You need two separate App Services
# Create both Backend and Frontend app services below
```

### 2. Create Azure App Services

#### Backend App Service (Required for both options)

```bash
# Set variables
RESOURCE_GROUP="my-resource-group"
APP_PLAN="my-app-plan"
BACKEND_APP="my-backend-app"

# Create App Service Plan
az appservice plan create \
  --name $APP_PLAN \
  --resource-group $RESOURCE_GROUP \
  --sku B1 \
  --is-linux

# Create Backend App Service
az webapp create \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_PLAN \
  --name $BACKEND_APP \
  --runtime "DOTNETCORE|10.0"

# Enable HTTPS only
az webapp update \
  --name $BACKEND_APP \
  --resource-group $RESOURCE_GROUP \
  --set httpsOnly=true
```

#### Frontend App Service (Only for Option 2 - Separate Deployment)

```bash
# Create Frontend App Service (if using separate deployment)
FRONTEND_APP="my-frontend-app"

az webapp create \
  --resource-group $RESOURCE_GROUP \
  --plan $APP_PLAN \
  --name $FRONTEND_APP \
  --runtime "node|24-lts"

# Enable HTTPS only
az webapp update \
  --name $FRONTEND_APP \
  --resource-group $RESOURCE_GROUP \
  --set httpsOnly=true
```

### 3. Download Publish Profiles

#### Backend Publish Profile

1. Go to **Azure Portal**
2. Navigate to **App Services** → your backend app
3. Click **Download Publish Profile** (right side, under Overview)
4. Save the file (it will be XML)

#### Frontend Publish Profile (Only for Option 2)

1. Go to **Azure Portal**
2. Navigate to **App Services** → your frontend app
3. Click **Download Publish Profile**
4. Save the file (it will be XML)

### 4. Add GitHub Secrets

1. Go to your GitHub repository on github.com
2. Click **Settings** tab
3. Click **Secrets and variables** → **Actions** → **New repository secret**

#### Add these secrets:

**For Option 1 (Combined - Minimum):**
```
AZURE_BACKEND_APP_NAME = your-backend-app
AZURE_BACKEND_PUBLISH_PROFILE = [paste entire XML content]
```

**For Option 2 (Separate - All of these):**
```
AZURE_BACKEND_APP_NAME = your-backend-app
AZURE_BACKEND_PUBLISH_PROFILE = [paste entire XML content]
AZURE_FRONTEND_APP_NAME = your-frontend-app
AZURE_FRONTEND_PUBLISH_PROFILE = [paste entire XML content]
```

**Important:** 
- Copy the ENTIRE publish profile XML (it's a multi-line XML file)
- Don't modify the XML content
- Keep the secret names exactly as shown

### 5. Update CORS Configuration

Edit `backend/Program.cs` and set your Azure frontend URL:

**For Option 1 (Combined):**
```csharp
// No additional CORS config needed - they're the same app
```

**For Option 2 (Separate):**
Update `appsettings.Production.json`:
```json
{
  "AzureFrontendUrl": "https://your-frontend-app.azurewebsites.net"
}
```

### 6. Choose and Enable Your Workflow

**Option 1: Combined Deployment**
- The workflow `.github/workflows/deploy.yml` is ready to use
- Just commit and push to `main` branch
- GitHub Actions will automatically run

**Option 2: Separate Deployment**
- Use workflow `.github/workflows/deploy-separate.yml` instead
- You can either:
  - Rename `deploy-separate.yml` to `deploy.yml` and delete original
  - Or disable `deploy.yml` and enable `deploy-separate.yml`

### 7. Test the Deployment

1. Commit your changes to the `main` branch:
```bash
git add .
git commit -m "Add GitHub Actions CI/CD workflows"
git push origin main
```

2. Go to GitHub → **Actions** tab
3. See your workflow running
4. Wait for it to complete (5-15 minutes)
5. Check Azure Portal → App Services → Activity Log

### 8. Verify Deployment

**For Combined Deployment:**
```bash
# Both frontend and backend are in one app
curl https://your-backend-app.azurewebsites.net/api/products
curl https://your-backend-app.azurewebsites.net/
```

**For Separate Deployment:**
```bash
# Backend API
curl https://your-backend-app.azurewebsites.net/api/products

# Frontend
curl https://your-frontend-app.azurewebsites.net/
```

## Workflow Details

### Combined Workflow (`deploy.yml`)

Triggers on: Push to `main` or Pull Request

**Steps:**
1. Checkout code
2. Setup .NET 10 and Node.js
3. Build frontend (Angular)
4. Copy frontend dist to backend's `wwwroot/`
5. Build backend (.NET)
6. Publish backend
7. Deploy backend (which includes frontend)

**Time:** ~10-15 minutes

### Separate Workflow (`deploy-separate.yml`)

Triggers on: Push to `main` or Pull Request

**Steps:**
1. Checkout code
2. Setup .NET 10 and Node.js
3. Build frontend and backend **in parallel**
4. Deploy backend
5. Deploy frontend **in parallel**

**Time:** ~10-15 minutes (parallel builds/deploys)

## Monitoring Deployments

### GitHub Actions Dashboard
- Go to **Actions** tab in your repository
- See all workflow runs
- Click on a run to see detailed logs
- Useful for debugging build/deploy issues

### Azure Portal
- Go to **App Services** → your app
- Click **Activity Log** to see deployment history
- Click **Deployment Center** for deployment settings
- Use **Diagnostic logs** for runtime issues

### View Logs
```bash
# Backend logs
az webapp log tail \
  --name your-backend-app \
  --resource-group your-resource-group

# Frontend logs (if separate)
az webapp log tail \
  --name your-frontend-app \
  --resource-group your-resource-group
```

## Troubleshooting

### Publish Profile Error
**Error:** "Invalid publish profile"

**Solution:**
- Re-download publish profile from Azure Portal
- Ensure you copied the entire XML (it's multi-line)
- Check secret name matches exactly: `AZURE_BACKEND_PUBLISH_PROFILE`
- Make sure there's no extra whitespace

### Deployment Fails with 404
**Error:** Frontend returns 404 after deployment

**Solutions:**
- For Combined: Check `wwwroot/index.html` exists after build
- For Separate: Verify `appsettings.Production.json` CORS is set correctly
- Check `MapFallbackToFile("index.html")` is in Program.cs

### API Calls Fail with CORS Error
**Error:** "No 'Access-Control-Allow-Origin' header"

**Solutions:**
- Verify CORS is configured in Program.cs
- Check `AzureFrontendUrl` in appsettings.Production.json
- For Option 1: CORS should already work (same origin)
- For Option 2: Ensure frontend URL matches exactly

### Build Timeout
**Error:** Workflow times out during build

**Solutions:**
- Increase Azure App Service plan to B2 or higher
- Clear npm cache: Delete `node_modules` and `package-lock.json`
- For .NET: `dotnet clean` before build

### Application Crashes on Startup
**Error:** App starts but then crashes

**Solutions:**
- Check Application Insights logs in Azure Portal
- Review `Diagnostic logs` → Application logs
- Ensure `appsettings.Production.json` is valid JSON

## Advanced Configuration

### Add Deployment Slots (Zero-Downtime Deploy)

Modify workflow to use staging slot:
```yaml
- name: Deploy to staging
  uses: azure/webapps-deploy@v2
  with:
    app-name: ${{ secrets.AZURE_BACKEND_APP_NAME }}
    publish-profile: ${{ secrets.AZURE_BACKEND_PUBLISH_PROFILE }}
    package: ./backend/publish/
    slot-name: staging

- name: Swap to production
  run: |
    az webapp deployment slot swap \
      --name ${{ secrets.AZURE_BACKEND_APP_NAME }} \
      --resource-group ${{ secrets.AZURE_RESOURCE_GROUP }} \
      --slot staging
```

### Deploy Only on Version Tags

```yaml
on:
  push:
    branches: [main]
    tags: ['v*']

jobs:
  deploy:
    if: startsWith(github.ref, 'refs/tags/v')
```

### Add Status Badges

Add to `README.md`:
```markdown
[![Deploy](https://github.com/YOUR_USERNAME/azure-webapp-poc/actions/workflows/deploy.yml/badge.svg)](https://github.com/YOUR_USERNAME/azure-webapp-poc/actions/workflows/deploy.yml)
```

## Security Best Practices

✅ **Following Best Practices:**
- Publish profiles stored as GitHub Secrets (encrypted)
- Secrets never logged or displayed
- HTTPS enforced on all connections
- Separate secrets for different environments
- Access control via repository settings

❌ **Never Do:**
- Commit publish profiles to repository
- Use same secrets for dev/staging/prod
- Hardcode app names
- Deploy on pull requests (use only on push to main)
- Share secrets outside of GitHub

## Performance Tips

### For Combined Deployment
- Use Azure Content Delivery Network (CDN) for static files
- Enable compression on App Service
- Use application insights for monitoring

### For Separate Deployment
- Use Azure CDN for frontend delivery
- Use Redis cache layer for backend
- Implement database connection pooling

## Cleanup

To remove GitHub Actions:
1. Delete `.github/workflows/deploy.yml` and `deploy-separate.yml`
2. Commit and push
3. Workflows will no longer run

To remove Azure Resources:
```bash
az group delete \
  --name my-resource-group \
  --yes
```

## Next Steps

1. ✅ Choose deployment option (Option 1 or 2)
2. ✅ Create Azure App Services
3. ✅ Download publish profiles
4. ✅ Add GitHub Secrets
5. ✅ Test deployment by pushing to main
6. ✅ Monitor Actions tab
7. ✅ Verify at azurewebsites.net URLs

## Resources

- [GitHub Actions Syntax](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions)
- [Azure WebApps Deploy Action](https://github.com/Azure/webapps-deploy)
- [Azure App Service Pricing](https://azure.microsoft.com/en-us/pricing/details/app-service/)
- [CORS Documentation](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS)

---

**Questions?** Check your workflow logs in the Actions tab or Azure Portal's Activity Log.
