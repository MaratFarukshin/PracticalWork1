@echo off
echo Starting Docker infrastructure...
docker-compose up -d
echo.
echo Waiting for services to start...
timeout /t 10 /nobreak
echo.
echo Infrastructure started!
echo.
echo Services:
echo - PostgreSQL Library: localhost:5432
echo - PostgreSQL Reports: localhost:5433
echo - Redis: localhost:6379
echo - MinIO: localhost:9000 (Console: localhost:9001)
echo - RabbitMQ: localhost:5672 (Management: localhost:15672)
echo - pgAdmin: localhost:5050
echo - Redis Commander: localhost:8081
echo.
pause

