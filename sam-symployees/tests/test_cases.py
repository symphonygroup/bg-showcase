from app.app import app
from fastapi.testclient import TestClient
from import_employees.app import lambda_handler
import boto3
import botocore
import moto
import time



env_s3_endpoint = "http://host.docker.internal:9000"

s3_endpoint = None
if not env_s3_endpoint == 'default':
  s3_endpoint = env_s3_endpoint

print('S3 Endpoint URL', s3_endpoint)

s3 = boto3.client('s3',
  endpoint_url=s3_endpoint
)


def client():
    return TestClient(
        app=app,
        base_url="http://localhost:8000",
    )


def test_sam_app():
    time.sleep(15)
    assert 1 is 1


def test_fastapi_app():
    res = client().get("/api")
    assert res.status_code == 200
    assert res.json() == {'Hello': '/'}
    time.sleep(6)


@moto.mock_sts
def test_lambda_handler():
    # Set up the mocked credentials
    sts_client = boto3.client("sts")
    assumed_role = sts_client.assume_role(
        RoleArn="arn:aws:iam::123456789012:role/test-role",
        RoleSessionName="test-session"
    )
    credentials = assumed_role["Credentials"]
    # Create an event object
    event = {
        'Records': [
            {
                's3': {
                    'bucket': {'name': 'test'},
                    'object': {'key': 'employees.csv'}
                }
            }
        ]
    }
    # Call the lambda handler with the event and credentials
    try:
        result = lambda_handler(event, credentials)
    except botocore.exceptions.ClientError as e:
        response = e.response
        assert response['Error']['Code'] == "AccessDenied"
