# dotnet-vue-ts-docker-sqlite-fullstack

## Running
```sh
# Contents in run.sh
docker build -f Dockerfile.dev -t dotnet-vue-ts-docker-sqlite-fullstack-dev .
docker compose -f docker-compose-dev.yml up
```

## Building
```sh
# Contents in build.sh
docker build -f Dockerfile -t dotnet-vue-ts-docker-sqlite-fullstack .
docker compose -f docker-compose.yml up
```

## Development journal
1. Install docker
2. Install dotnet
3. Install node + npm, and enable corepack
```sh
# After installing node
sudo corepack enable
```
4. Create new dotnet minimal API project, for backend
```sh
dotnet new web -o backend
```
5. Install Entity Framework tool
```sh
# In backend folder, run
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```
6. Add dotnet tools to path
```sh
# ~/.bashrc
export PATH="$PATH:/home/valian/.dotnet/tools"
```
7. Create vue 3 project
```sh
npm i -g @vue/cli
vue create frontend
# manually select features
# - typescript
# - vue 3.x
# - don't use class-style component syntax
# - don't use babel alongside typescript
# - put config files in dedicated config files
```