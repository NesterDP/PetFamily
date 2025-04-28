docker exec postgres psql -U postgres -c "DROP DATABASE IF EXISTS hangfire;"
docker exec postgres psql -U postgres -c "CREATE DATABASE hangfire;"

docker exec mongodb mongosh "mongodb://mongoadmin:mongopassword@localhost:27017/?authSource=admin" --eval "db.getSiblingDB('files_db').dropDatabase()"
docker exec mongodb mongosh "mongodb://mongoadmin:mongopassword@localhost:27017/?authSource=admin" --eval "db = db.getSiblingDB('files_db'); db.createCollection('files')"