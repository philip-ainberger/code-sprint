#!/bin/sh
cat <<EOF > /usr/share/nginx/html/assets/config/config.production.json
{
    "apiBaseUrl": "${API_BASE_URL}"
}
EOF

echo "Configuration file has been created successfully."

# Start Nginx in the foreground
nginx -g 'daemon off;'