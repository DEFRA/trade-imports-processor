#!/bin/bash

export AWS_ENDPOINT_URL=http://localhost:4566
export AWS_REGION=eu-west-2
export AWS_DEFAULT_REGION=eu-west-2
export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test

aws --endpoint-url=http://localhost:4566 sqs create-queue \
    --queue-name trade_imports_inbound_customs_declarations_processor.fifo \
    --attributes FifoQueue=true

aws --endpoint-url=http://localhost:4566 sqs create-queue \
    --queue-name trade_imports_inbound_customs_declarations_processor-deadletter.fifo \
    --attributes FifoQueue=true
    
aws --endpoint-url=http://localhost:4566 sqs create-queue \
    --queue-name trade_imports_data_upserted_processor
    
aws --endpoint-url=http://localhost:4566 sqs create-queue \
    --queue-name trade_imports_data_upserted_processor-deadletter

aws --endpoint-url=http://localhost:4566 sqs set-queue-attributes --queue-url "http://localhost:4566/000000000000/trade_imports_data_upserted_processor" --attributes '{"RedrivePolicy": "{\"deadLetterTargetArn\":\"arn:aws:sqs:eu-west-2:000000000000:trade_imports_data_upserted_processor-deadletter\",\"maxReceiveCount\":\"1\"}"}'

aws --endpoint-url=http://localhost:4566 sqs set-queue-attributes --queue-url "http://localhost:4566/000000000000/trade_imports_inbound_customs_declarations_processor.fifo" --attributes '{"RedrivePolicy": "{\"deadLetterTargetArn\":\"arn:aws:sqs:eu-west-2:000000000000:trade_imports_inbound_customs_declarations_processor-deadletter.fifo\",\"maxReceiveCount\":\"1\"}"}'

function is_ready() {
    aws --endpoint-url=http://localhost:4566 sqs get-queue-url --queue-name trade_imports_inbound_customs_declarations_processor.fifo || return 1
    aws --endpoint-url=http://localhost:4566 sqs get-queue-url --queue-name trade_imports_inbound_customs_declarations_processor-deadletter.fifo || return 1
    aws --endpoint-url=http://localhost:4566 sqs get-queue-url --queue-name trade_imports_data_upserted_processor || return 1
    aws --endpoint-url=http://localhost:4566 sqs get-queue-url --queue-name trade_imports_data_upserted_processor-deadletter || return 1
    return 0
}

while ! is_ready; do
    echo "Waiting until ready"
    sleep 1
done

touch /tmp/ready