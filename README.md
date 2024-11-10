# 📋 To Do List
A simple To Do List application built with the following technologies:
- **C#** & **Entity Framework**
- **React** & **TypeScript**
- **PostgreSQL**
- **Docker**

---
## How To Start

### [Windows Setup Guide](#windows-setup-guide) 🪟
### [Linux Setup Guide](#linux-setup-guide) 🐧
### [API DOCUMENTATION](#api-documentation) 📝
### [Overview](#overview) 🔍
- [API](#api)🔗
- [API TESTS](#api-tests) 🧪
- [UI](#ui) 🖼️
- [Docker](#docker) 📦
-----

## Windows Setup Guide  
### 0. Prerequisites
 **Docker Desktop**: Download and install Docker Desktop for Windows from [Docker’s official website](https://www.docker.com/products/docker-desktop/).
  Ensure Docker Desktop is running. You should see the Docker icon in your system tray.

### 1. Open Docker Desktop
   - Open Docker Desktop and ensure it’s running.
   - Confirm Docker is set to use **Linux containers**. You can check this in the Docker Desktop settings or by clicking Docker icon in your system tray.


### 2. Open a Terminal
   - You can use **Command Prompt**, **PowerShell**, or **Windows Terminal** *(make sure you start with admin privileges)*.
   - Navigate to the project directory containing the `docker-compose.yml` file.
   - Example:
     ```bash
     cd path\to\your\project
     ```

### 3. Start the Docker-Compose Project
   - Run the following command to start the services defined in the `docker-compose.yml` file:
     ```bash
     docker compose up --build
     ```
   - This will download any necessary images and start the containers as per the configuration in the `docker-compose.yml` file.
    ```
### 4. Verify Running Containers
   - After starting, you can check the running containers using:
     ```bash
     docker-compose ps
     ```
   - The List should include the following container:
        1. frontend_1
        2. backend_1
        3. db_1

### 5. Stopping the Containers
   - To stop all running containers, use:
     ```bash
     docker-compose down
     ```

### Troubleshooting Tips
- **Port Conflicts**: Make sure the following ports are free on your system: 
    - tcp/0.0.0.0:5432 
    - localhost:5173
    - localhost:5180
- **Permission**: Make sure Docker Desktop has the right permission to be able to virtualize on your system, also start your terminal with admin privileges
---

## Linux Setup Guide  

## 0. Prerequisites

- **Docker**: Install Docker on your Linux system by following the [official Docker installation guide for Linux](https://docs.docker.com/engine/install/).
- **Docker Compose**: Install Docker Compose by following the [official Docker Compose installation guide](https://docs.docker.com/compose/install/).
- Ensure the Docker service is running. You can verify this by running:

    ```bash
    sudo systemctl status docker
    ```

- You may need to add your user to the Docker group to run Docker commands without `sudo`. To do this, run:

    ```bash
    sudo usermod -aG docker $USER
    ```

    After running this command, log out and log back in for the group changes to take effect.

---

## 1. Open a Terminal

- Open your terminal application (e.g., GNOME Terminal, Konsole, etc.).
- Verify that Docker and Docker Compose are installed by running:

    ```bash
    docker --version
    docker-compose --version
    ```

- Navigate to the project directory that contains the `docker-compose.yml` file:

    ```bash
    cd /path/to/your/project
    ```

---

## 2. Start the Docker-Compose Project

- To start the services defined in the `docker-compose.yml` file, run the following command in the root of the project:

    ```bash
    docker-compose up --build
    ```

    This will download any necessary images and start the containers according to the configuration in the `docker-compose.yml` file.

---

## 3. Verify Running Containers

- After starting the containers, you can check the running containers using:

    ```bash
    docker-compose ps
    ```

    The list should include the following containers (or similar):

    ```
    frontend_1
    backend_1
    db_1
    ```

---

## 4. Stopping the Containers

- To stop all running containers, use the following command:

    ```bash
    docker-compose down
    ```

---

## Troubleshooting Tips

- **Port Conflicts**: Make sure the following ports are free on your system:
    - `tcp/0.0.0.0:5432`
    - `localhost:5173`
    - `localhost:5180`

- **Permission Issues**: Make sure Docker has the appropriate permissions to virtualize on your system. Also, ensure your terminal has the necessary privileges (e.g., using `sudo` if required).

- **Docker Group**: If you encounter permission errors, ensure your user is added to the Docker group and you've logged out and back in for the changes to take effect.

---

## API Documentation
API Documentation can be found directly when visiting localhost:5180(while app is running)

## Overview

### **API**

The `API` directory contains the server-side application built with .NET, including database access, API endpoints, services, and configuration.

#### - `Endpoints/`
Contains API route handlers that define the application’s RESTful endpoints.

- **`ToDoTasks.cs`**: Defines endpoints related to ToDo tasks, such as getting tasks, creating new tasks, updating or deleting tasks, etc.

#### - `Helpers/`
Contains utility classes and extensions for simplifying code or adding reusable functionality.

- **`QueryableExtensions.cs`**: Extension methods for `IQueryable`, likely used for filtering or querying data in a more readable or reusable way.

#### - `Services/`
Contains business logic and services that interact with the models and manage application workflows.


#### - `Configuration.cs`
A Configuration abstraction using `Microsoft.Configuration.Extensions`

---

### **API Tests**

The `API.Tests` directory contains unit and integration tests for the backend API. It is responsible for ensuring that the API logic behaves correctly and meets requirements.

#### - `ToDoService/`
- `GeneralTests.cs` - General Service Tests - CRUD

The Following tests are related to each Specific method of the ToDoService and have 100% coverage:
- `GetAll.cs`
- `GetCompleted.cs`
- `GetOverdue.cs`
- `GetPending.cs`
- `GetPending`

---

### **UI**

The `UI` directory contains the frontend React + TypeScript application, including UI components, hooks, services, and other frontend logic.

#### - `components/`
Contains reusable UI components used throughout the application.

- **`AddButton/`**: A button component that triggers an action to add a new task or item.
- **`AddForm/`**: A form component used to gather data for creating a new ToDo task.
- **`AsyncConfirmation/`**: A confirmation dialog component that handles asynchronous actions, like confirming the deletion of a task.
- **`DeleteButton/`**: A button component that triggers the deletion of a ToDo task.

#### - `index.css`
Global CSS styles for the frontend application. This file defines the general styling and layout of the app.

#### - `enums/`
- **`SortGroup.ts`**: Enum for handling sorting groups (e.g., "alphabetical," "date").
- **`Status.ts`**: Enum for handling Service Statuses 
- **`TaskStatus.ts`**: Enum for task statuses (e.g., "Completed," "Pending").

#### - `hooks/`
- **`useFormChange.tsx`**: A hook for managing form state changes.
- **`useLoadingSpinner.tsx`**: A hook for showing and hiding a loading spinner based on asynchronous state.
- **`useLocalStorage.tsx`**: A hook for interacting with the browser's local storage.
- **`useOutsideClick.tsx`**: A hook that detects clicks outside a specified element.
- **`useToDoCache.tsx`**: A hook that handles caching ToDo data for improved performance.

#### - `services/`
- **`request.ts`**: Abstraction similar to axios.


---

### **Docker**

#### - `Dockerfile.ui` & `Docker.api`
Dockerfiles for containerizing the frontend and backend respectively, *db is configured in docker-compose.yml*, similar to the but for the React frontend.

#### - `docker-compose.yml`
A Docker Compose configuration file used for defining and running multi-container Docker applications, including both the API and UI.

---

## Summary

This structure follows best practices for full-stack development, with clear separation of concerns between the frontend, backend, and testing layers.
