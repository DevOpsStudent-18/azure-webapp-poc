# GitHub Actions CI/CD - Setup Summary

This document summarizes the CI/CD setup for deploying to Azure App Service.

## What Was Added

### 📁 Files Created

```
.github/
├── workflows/
│   ├── deploy.yml                    ✓ Main workflow (Combined deployment)
│   └── deploy-separate.yml           ✓ Alternative workflow (Separate deployment)
└── WORKFLOWS_README.md               ✓ Complete setup guide

Root directory:
├── DEPLOYMENT.md                     ✓ Detailed deployment instructions
└── appsettings.Production.json       ✓ Production configuration template
```

### 🔄 Updated Files

- `backend/Program.cs` - Added static file serving and SPA fallback for Azure
- `backend/appsettings.json` - Added CORS configuration
- `README.md` - Added CI/CD section

## 🚀 Quick Start for Deployment

### Step 1: Choose Your Deployment Strategy

**Option A: Combined Deployment (Recommended)** ✓
- Use: `.github/workflows/deploy.yml`
- Cost: 💰💰 (Single app service)
- Complexity: 🟢 Easy
- Best for: Most projects

**Option B: Separate Deployment** 
- Use: `.github/workflows/deploy-separate.yml`
- Cost: 💰💰💰 (Two app services)
- Complexity: 🟡 Medium
- Best for: Large scale projects needing independent scaling

### Step 2: Create Azure Resources

```bash
# Using Azure CLI
az group create --name my-rg --location eastus

# Create App Service Plan
az appservice plan create \
  --name my-plan \
  --resource-group my-rg \
  --sku B1 --is-linux

# Create Backend App Service
az webapp create \
  --name my-backend-app \
  --resource-group my-rg \
  --plan my-plan \
  --runtime "DOTNETCORE|10.0"

# For Option B only - Create Frontend App Service
az webapp create \
  --name my-frontend-app \
  --resource-group my-rg \
  --plan my-plan \
  --runtime "node|24-lts"
```

### Step 3: Get Publish Profiles

1. Go to Azure Portal → App Services → Your app
2. Click "Download Publish Profile" button
3. Copy entire XML content

### Step 4: Add GitHub Secrets

**Minimum secrets (Option A):**
```
AZURE_BACKEND_APP_NAME = my-backend-app
AZURE_BACKEND_PUBLISH_PROFILE = [paste full XML]
```

**All secrets (Option B):**
```
AZURE_BACKEND_APP_NAME = my-backend-app
AZURE_BACKEND_PUBLISH_PROFILE = [paste backend XML]
AZURE_FRONTEND_APP_NAME = my-frontend-app
AZURE_FRONTEND_PUBLISH_PROFILE = [paste frontend XML]
```

### Step 5: Update CORS (Option B only)

Edit `backend/appsettings.Production.json`:
```json
{
  "AzureFrontendUrl": "https://my-frontend-app.azurewebsites.net"
}
```

### Step 6: Test Deployment

```bash
git add .
git commit -m "Add GitHub Actions CI/CD"
git push origin main
```

Watch the Action tab → see your workflow run → monitor logs

### Step 7: Verify

```bash
# For Option A
curl https://my-backend-app.azurewebsites.net/api/products

# For Option B
curl https://my-backend-app.azurewebsites.net/api/products
curl https://my-frontend-app.azurewebsites.net/
```

## 📋 Checklist

- [ ] Read [.github/WORKFLOWS_README.md](.github/WORKFLOWS_README.md)
- [ ] Create Azure resource group and plan
- [ ] Create App Service(s)
- [ ] Download publish profile(s)
- [ ] Add GitHub secrets
- [ ] (Option B only) Update appsettings.Production.json
- [ ] Push to main branch
- [ ] Monitor Actions tab
- [ ] Verify deployment at azurewebsites.net

## 🔍 Workflow Overview

### Combined Deployment Flow (Option A)

```
Push to main
    ↓
[GitHub Actions]
    ├─ Setup .NET & Node
    ├─ Build Angular frontend
    ├─ Copy frontend → backend/wwwroot
    ├─ Build .NET backend
    └─ Deploy to Azure App Service
    ↓
Single App Service
    ├─ /api/products (API)
    └─ / (Frontend - served as static files)
```

### Separate Deployment Flow (Option B)

```
Push to main
    ↓
[GitHub Actions]
    ├─ Build Angular frontend
    ├─ Build .NET backend
    ├─ Deploy backend → Backend App Service
    └─ Deploy frontend → Frontend App Service
    ↓
Backend App Service       Frontend App Service
    └─ /api/products           └─ /
```

## 📚 Documentation Files

- **[README.md](README.md)** - Main project documentation
- **[QUICKSTART.md](QUICKSTART.md)** - Local development quick start
- **[DEPLOYMENT.md](DEPLOYMENT.md)** - Detailed deployment documentation
- **[.github/WORKFLOWS_README.md](.github/WORKFLOWS_README.md)** - GitHub Actions complete guide
- **[.github/workflows/deploy.yml](.github/workflows/deploy.yml)** - Combined deployment workflow
- **[.github/workflows/deploy-separate.yml](.github/workflows/deploy-separate.yml)** - Separate deployment workflow

## 🛠️ Troubleshooting

### "Publish profile is invalid"
- Download fresh profile from Azure Portal
- Paste entire XML (it's multi-line)
- Check secret name exactly matches

### "Frontend returns 404 after deployment"
- Check `index.html` in backend/wwwroot/ (Option A)
- Verify CORS in appsettings.Production.json (Option B)
- Run build manually: `npm run build` in frontend/

### "API calls fail with CORS error"
- Option A: Should work automatically (same origin)
- Option B: Verify `AzureFrontendUrl` matches exactly

### "Deployment times out"
- Use B2 or higher App Service plan
- Check build logs in Actions tab
- Try clearing cache: `rm -rf node_modules package-lock.json`

## 🔐 Security Notes

✅ **Good:**
- Publish profiles in GitHub Secrets (encrypted)
- Different profiles for dev/staging/prod
- HTTPS enforced
- Secrets not logged

❌ **Never:**
- Commit publish profiles
- Use same profile for all environments
- Display secrets in logs
- Share secrets outside GitHub

## 📊 Monitoring

### GitHub Actions
- Go to Actions tab
- Click workflow run
- View detailed build/deploy logs
- Check for errors or warnings

### Azure Portal
- App Services → Activity Log
- Deployment Center
- Diagnostic Logs
- Application Insights (if enabled)

## 💡 Tips

1. **Test locally first**
   ```bash
   dotnet run          # Backend
   npm start          # Frontend (new terminal)
   ```

2. **Watch the logs**
   - GitHub Actions: Real-time build logs
   - Azure Portal: Deployment status

3. **Use deployment slots** (Advanced)
   - Staging slot for testing
   - Swap to production for zero-downtime

4. **Monitor costs**
   - B1 plan: ~$50/month (sufficient for POC)
   - Consider auto-scale for production

## 🎯 Next Steps

1. **Immediate:** Follow the Quick Start (Step 1-7 above)
2. **Short-term:** Test deployment and verify
3. **Medium-term:** Set up monitoring and alerts
4. **Long-term:** Add database, authentication, testing

## 📞 Support Resources

- [GitHub Actions Docs](https://docs.github.com/en/actions)
- [Azure App Service Docs](https://learn.microsoft.com/en-us/azure/app-service/)
- [Azure WebApps Deploy Action](https://github.com/Azure/webapps-deploy)

## 🎓 Learning Resources

- Understanding CI/CD: https://en.wikipedia.org/wiki/CI/CD
- GitHub Actions: https://github.com/features/actions
- Azure App Service: https://azure.microsoft.com/en-us/services/app-service/
- Publishing .NET apps: https://learn.microsoft.com/en-us/dotnet/core/deploying/

---

**Ready?** Start with [.github/WORKFLOWS_README.md](.github/WORKFLOWS_README.md) for complete setup instructions!
