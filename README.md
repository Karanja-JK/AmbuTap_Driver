# AmbuTap Driver

A real-time ambulance driver application built for the AmbuTap emergency medical response system. This Android application enables ambulance drivers to receive, manage, and complete emergency medical transport requests efficiently.

Why AmbuTap? Ambulance(Ambu) at the (Tap) of your screen - AmbuTap. Cool right?

## Overview

AmbuTap Driver is part of a comprehensive ambulance hailing system designed to address Kenya's emergency medical response challenges. The application provides ambulance drivers with tools to manage trip requests, navigate to patients, and coordinate with the central dispatch system.

This system addresses the critical gap in emergency medical response where the average response time is 2-3 hours compared to the WHO recommended 15-20 minutes.

The AmbuTap Ecosystem.
The full system includes several distinct components working together:
- AmbuTap User App: The patient-facing application for requesting an ambulance.
- AmbuTap Driver App: This application, for use by ambulance drivers.
- Admin Dashboard: A web-based panel for managing drivers, users, and system operations.
- AI Health Bot: A chatbot integrated to provide users with basic medical information and assistance.

## Features

### Core Functionality
- **Real-time Trip Requests**: Receive instant notifications for nearby emergency requests
- **Trip Management**: Accept or decline incoming requests based on availability
- **Live Location Tracking**: Share real-time location with dispatch and patients
- **Route Optimization**: Integrated Google Maps for efficient navigation to pickup and destination points
- **Trip Status Management**: Start, update, and complete trips with status tracking
- **Payment Processing**: Handle trip payments and view earnings history

### Driver Operations
- **Availability Control**: Toggle online/offline status to manage work hours
- **Driver Verification**: Admin-approved registration system for verified drivers only
- **Patient Communication**: Direct contact capabilities with patients during trips
- **Trip History**: Complete record of all completed and cancelled trips
- **Profile Management**: Update driver information and vehicle details

### Safety and Security
- **Identity Verification**: Comprehensive driver background verification through admin panel
- **Secure Communication**: Encrypted messaging between drivers and patients
- **Emergency Protocols**: Built-in safety features for high-risk situations
- **Data Protection**: Secure handling of sensitive medical and personal information

## Technical Architecture

### Technology Stack
- **Platform**: Xamarin Android
- **Language**: C#
- **Database**: Firebase Realtime Database
- **Maps**: Google Maps Android API
- **Authentication**: Firebase Authentication
- **Real-time Communication**: Firebase Cloud Messaging
- **Development Environment**: Visual Studio 2019

### Project Structure
```
AmbuTap Driver/
├── Activities/          # Application screens and user interfaces
├── Adapter/            # List adapters for data display
├── Assets/             # Static assets and configuration files
├── DataModels/         # Data structures for trips, drivers, and locations
├── EventListener/      # Real-time event handling for Firebase
├── Fragments/          # Reusable UI components
├── Helpers/            # Utility classes for GPS, networking, and data processing
├── Resources/          # Android resources (layouts, strings, drawables)
├── Properties/         # Project configuration and assembly info
├── MainActivity.cs     # Application entry point
└── google-services.json # Firebase configuration
```

### Key Components

#### Data Models
The application uses structured data models for:
- **Trip Information**: Request details, pickup/destination coordinates, patient information
- **Driver Profiles**: Personal information, vehicle details, verification status
- **Location Data**: Real-time GPS coordinates and route information
- **Payment Records**: Trip costs, payment methods, and transaction history

#### Event Listeners
Real-time event handling system manages:
- Incoming trip requests from the central system
- Location updates from GPS services
- Status changes from patient applications
- Communication messages between drivers and dispatch

#### Helper Classes
Utility functions provide:
- GPS coordinate processing and route calculations
- Network communication with Firebase backend
- Data formatting for display and storage
- Payment processing and validation

## Installation and Setup

### Prerequisites
- Android device running Android 5.0 (API level 21) or higher
- Valid driver's license and ambulance operation certification
- Admin approval for driver account activation

### Development Setup
1. Clone the repository
2. Open the project in Visual Studio 2019 or later
3. Install required NuGet packages:
   - Xamarin.Android.Support libraries
   - Xamarin.GooglePlayServices.Maps
   - Xamarin.Firebase packages
4. Add your `google-services.json` file to the project root
5. Configure Firebase project with appropriate API keys
6. Build and deploy to Android device or emulator

### Driver Registration Process
1. Download and install the AmbuTap Driver application
2. Complete registration with required documentation:
   - Valid driver's license
   - Ambulance operation certification
   - Vehicle registration and insurance
   - Emergency medical technician certification
3. Wait for admin verification and account approval
4. Receive approval notification and begin operations

## Usage

### Getting Started
1. **Account Setup**: Complete registration and wait for admin approval
2. **Profile Configuration**: Add vehicle details and emergency contact information
3. **Go Online**: Toggle availability to start receiving trip requests
4. **Location Services**: Ensure GPS is enabled for accurate positioning

### Managing Trip Requests
1. **Receiving Requests**: New trip notifications appear with patient location and details
2. **Accept/Decline**: Review request information and respond within the timeout period
3. **Navigation**: Use integrated maps to navigate to pickup location
4. **Patient Contact**: Communicate with patients for specific instructions or updates

### Completing Trips
1. **Start Trip**: Confirm patient pickup and begin journey to destination
2. **Status Updates**: Provide real-time updates on trip progress
3. **Destination Arrival**: Confirm safe delivery of patient to medical facility
4. **Payment**: Process payment and complete trip documentation

## Configuration

### Firebase Setup
The application requires proper Firebase configuration:
- Realtime Database for trip and driver data
- Authentication for secure user management
- Cloud Messaging for push notifications
- Storage for document and image uploads

### Google Maps Integration
- Enable Google Maps Android API in Google Cloud Console
- Configure API keys for location services and mapping
- Set up Places API for address autocomplete functionality

### Permissions
The application requires the following Android permissions:
- Location services (fine and coarse location)
- Network access for data synchronization
- Phone access for emergency calling
- Camera access for document verification
- Storage access for offline data caching

## System Requirements

### Hardware Requirements
- Android device with GPS capabilities
- Minimum 2GB RAM for optimal performance
- Stable internet connection (3G/4G/WiFi)
- Camera for document verification

### Software Requirements
- Android 5.0 (API level 21) or higher
- Google Play Services installed and updated
- Sufficient storage space for offline maps and data

## Testing

The application underwent comprehensive testing including:

### Unit Testing
- Individual component functionality verification
- Data model validation and error handling
- Network communication reliability testing

### Integration Testing
- Firebase database synchronization
- Google Maps API integration
- Real-time messaging system validation

### User Acceptance Testing
- Field testing with emergency medical technicians
- Performance testing under various network conditions
- Usability testing for high-stress emergency situations

## Contributing

This project was developed as an academic final year project. While the primary development phase is complete, the codebase serves as a reference for emergency medical response system development.

### Development Guidelines
- Follow C# coding conventions and best practices
- Maintain comprehensive error handling for network operations
- Implement proper data validation for all user inputs
- Ensure compliance with medical data privacy regulations

## License

This project was developed as an academic submission for the University of Nairobi School of Computing and Informatics. The code is provided for educational and reference purposes.

## Acknowledgments

**Project Supervision**: Dr. Eng. Lawrence Muchemi, University of Nairobi

**Research Contributors**: Emergency Response Team officers from St. Johns Ambulance and Mercy Light Hospital, Kiambu

**Technical Inspiration**: Analysis of existing emergency response applications including VMEDO, Meddco Ambulance, and Flare App

**Academic Institution**: University of Nairobi, College of Biological and Physical Sciences, School of Computing and Informatics

## Project Context

This application addresses critical challenges in Kenya's emergency medical response system:
- Average response time of 2-3 hours vs WHO recommended 15-20 minutes
- Less than 1,000 ambulances nationwide for a population requiring significantly more
- Unreliable emergency hotline (999) forcing people to use personal transportation
- Lack of organized national emergency care coordination

The AmbuTap system aims to bridge these gaps through technology-driven solutions while working within existing healthcare regulations and infrastructure.

## Related Projects

- **AmbuTap User App**: Patient-facing application for requesting ambulances
- **AmbuTap Admin Dashboard**: Web-based management system for operators
- **AmbuTap Health Bot**: AI-driven medical assistance chatbot

## Contact

**Developer**: Jesse Karanja Ng'ang'a  
**Institution**: University of Nairobi  
**Project Year**: 2020  
**Supervisor**: Dr. Eng. Lawrence Muchemi
