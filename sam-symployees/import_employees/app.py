import urllib.parse
import boto3
import os

print('Loading function')

env_s3_endpoint = os.getenv('S3_ENDPOINT_URL')

s3_endpoint = None
if not env_s3_endpoint == 'default':
  s3_endpoint = env_s3_endpoint

print('S3 Endpoint URL', s3_endpoint)

s3 = boto3.client('s3', 
  endpoint_url=s3_endpoint
)


def lambda_handler(event, context):
    #print("Received event: " + json.dumps(event, indent=2))

    # Get the object from the event and show its content type
    bucket = event['Records'][0]['s3']['bucket']['name']
    key = urllib.parse.unquote_plus(event['Records'][0]['s3']['object']['key'], encoding='utf-8')
    try:
        response = s3.get_object(Bucket=bucket, Key=key)
        print("CONTENT TYPE: " + response['ContentType'])
        return response['ContentType']
    except Exception as e:
        print(e)
        print('Error getting object {} from bucket {}. Make sure they exist and your bucket is in the same region as this function.'.format(key, bucket))
        raise e