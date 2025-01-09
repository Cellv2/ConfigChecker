# note: if strange things start happening with this startup script (sleep 1\r for example), ensure it's using LF and not CRLF

# got to start the server first
redis-server --daemonize yes && sleep 1

# https://redis.io/learn/guides/import
redis-cli < ./src/seed-data.redis
redis-cli save
redis-cli shutdown
redis-server