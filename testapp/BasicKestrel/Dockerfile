FROM microsoft/dotnet:1.0.0-rc2-core

# Set the Working Directory
WORKDIR /app

# Configure the listening port to 5000
#ENV ASPNETCORE_URLS http://+:5000
#EXPOSE 5000

# Copy the app
COPY .testpublish/ /app

# Start the app
ENTRYPOINT dotnet BasicKestrel.dll
