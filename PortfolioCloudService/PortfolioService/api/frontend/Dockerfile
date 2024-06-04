# Use a newer Node.js LTS image as the base image
FROM node:20-alpine

# Set the working directory inside the container
WORKDIR /app

# Copy package.json and package-lock.json to the working directory
COPY package*.json ./

# Install dependencies
RUN npm install --legacy-peer-deps

# Copy the entire project to the container
COPY . .

# Build your
RUN npm run build

# Expose the port your app runs on
EXPOSE 3000

# Set environment variable for running in production
ENV NODE_ENV=production

# Start your application
CMD ["npm", "start"]
