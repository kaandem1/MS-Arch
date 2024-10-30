# Use the official Node.js image with a compatible version as the base image
FROM node:20 AS build

# Set the working directory
WORKDIR /app

# Copy the package.json and package-lock.json
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy the rest of the application code
COPY . .

# Build the application for production
RUN npm run build --prod

# Use Nginx to serve the application
FROM nginx:1.17-alpine

# Install curl for fetching envsubst
RUN apk --no-cache add curl && \
    curl -L https://github.com/a8m/envsubst/releases/download/v1.2.0/envsubst-`uname -s`-`uname -m` -o /usr/local/bin/envsubst && \
    chmod +x /usr/local/bin/envsubst

# Copy the nginx configuration template
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy the build output to Nginx's html directory
COPY --from=build /app/dist/kaanergize-ng/browser /usr/share/nginx/html/

# Expose port 80
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]

