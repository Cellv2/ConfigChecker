FROM redis:7.4

WORKDIR /src
COPY ["/Redis/seed-data.redis", "."]
COPY ["/Redis/start-redis.sh", "."]

WORKDIR /

CMD ["sh", "src/start-redis.sh"]