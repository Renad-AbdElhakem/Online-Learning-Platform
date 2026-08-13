# Online Course Platform

An online course platform built with a **Microservices Architecture** using .NET.

## Overview

This project simulates a real-world e-learning platform where instructors manage course content, students interact with lectures and assignments, and admins oversee course and group operations — all built across independent, communicating services.

## Features

### Content Management
- Upload, download, and view lecture videos
- Upload, download, and view course materials (PDF)

### Instructor
- Upload and edit lecture videos and materials

### Student
- Submit assignments for grading

### Instructor (Grading)
- Grade submitted student assignments

### Admin
- Create and manage courses
- Create and manage groups
- Assign instructors to groups
- Track group capacity and student count

## Architecture & Tech Stack

- **Minimal APIs** — lightweight service endpoints
- **API Gateway** — single entry point for routing requests to the appropriate services
- **Service Discovery** — dynamic registration and discovery of services
- **Health Checks (Consul)** — service health monitoring for the gateway/discovery layer
- **Rate Limiting** — traffic control at the gateway
- **HttpClient** — inter-service communication

## Project Structure

Each capability (content upload/management, assignments/grading, course & group administration) is implemented as an independent service, communicating with the others through HTTP calls routed via the API Gateway, and registered through Service Discovery with health checks in place.

## Getting Started

> Add your actual setup steps here (clone, restore, run each service, gateway configuration, etc.)

```bash
git clone <repo-url>
cd online-course-platform
# restore & run instructions
```

## License

This project is for educational and portfolio purposes.
