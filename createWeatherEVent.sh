curl -d '{"locationName":"Brooklyn, NY", "temperature":91,
  "timestamp":1564428897, "latitude": 40.70, "longitude": -73.99}' \
  -H "Content-Type: application/json" \
  -X POST https://jn1ly3l947.execute-api.us-east-1.amazonaws.com/Prod/events 
