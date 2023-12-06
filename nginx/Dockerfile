# Use the Nginx image from Docker Hub
FROM nginx:latest

# # Remove the default Nginx configuration file
# RUN rm /etc/nginx/conf.d/default.conf

# # Add a new Nginx configuration file
# COPY nginx.conf /etc/nginx/conf.d/

RUN rm /etc/nginx/nginx.conf

COPY nginx.conf /etc/nginx/nginx.conf