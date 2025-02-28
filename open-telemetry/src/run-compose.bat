@echo off
for /f "delims=" %%i in ('aws configure get aws_access_key_id --profile default') do set AWS_ACCESS_KEY_ID=%%i
for /f "delims=" %%i in ('aws configure get aws_secret_access_key --profile default') do set AWS_SECRET_ACCESS_KEY=%%i

docker compose up
