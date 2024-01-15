# Twitter Backend API

This repository contains the backend API for a Twitter-like application built with ASP.NET Core, SignalR, Redis, and PostgreSQL. The API provides a set of features commonly found in social media platforms, including authentication, registration, tweets, timeline, follow/unfollow, like, retweet, real-time notifications, and analytics.

## Features

### 1. Authentication and Registration

- **Login**: Users can log in using their credentials.
- **Registration**: New users can register with a valid email and password.

### 2. Tweets and Timeline

- **Create Tweet**: Users can create and post tweets.
- **Timeline**: Users have a personalized timeline that displays tweets from the users they follow.

### 3. Follow/Unfollow

- **Follow**: Users can follow other users to see their tweets in their timeline.
- **Unfollow**: Users can unfollow others to stop seeing their tweets.

### 4. Like and Retweet

- **Like**: Users can like tweets to show appreciation.
- **Retweet**: Users can share tweets they find interesting with their followers.

### 5. Real-Time Notifications

- **Real-Time Updates**: Users receive real-time notifications for new tweets, likes, retweets, and follows.

### 6. Analytics

- **User Analytics**: Track user engagement, such as the number of tweets, followers, and likes.

## Technologies Used

- **ASP.NET Core**: The primary framework for building the API.
- **SignalR**: Used for real-time communication between the server and clients.
- **Redis**: Used for caching and real-time updates.
- **PostgreSQL**: Database for storing user data, tweets, and analytics.

## Getting Started

To run the Twitter Backend API locally, follow these steps:

1. **Prerequisites:**

   - Install [.NET Core](https://dotnet.microsoft.com/download).
   - Set up a PostgreSQL database and update the connection string in the `appsettings.json` file.
   - Ensure Redis is installed and running.
2. **Clone the Repository:**

   ```bash
   git clone https://github.com/your-username/twitter-backend-api.git
   cd twitter-backend-api
   ```
3. **Run Migrations:**

   ```bash
   dotnet ef database update
   ```
4. **cd  into src folder and Run the Application:**

   ```bash
   dotnet run
   ```
5. **Open in Browser:**
   Visit `https://localhost:5001` in your browser.

## API Documentation

For detailed information about the API endpoints and usage, refer to the [API documentation](docs/API_DOCUMENTATION.md).

## Contributing

Contributions are welcome! Please follow the [Contribution Guidelines](CONTRIBUTING.md) to contribute to the project.

## License

This project is licensed under the [MIT License](LICENSE).
