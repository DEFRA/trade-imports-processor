#!/bin/bash
awslocal sqs create-queue --queue-name trade_gateway_publisher_ched_updates_processor-deadletter.fifo --attributes FifoQueue=true
awslocal sqs create-queue --queue-name trade_gateway_publisher_ched_updates_processor.fifo --attributes '{"FifoQueue":"true","RedrivePolicy": "{\"deadLetterTargetArn\":\"arn:aws:sqs:eu-west-2:000000000000:trade_imports_inbound_customs_declarations_processor-deadletter.fifo\",\"maxReceiveCount\":\"1\"}"}'

awslocal sqs create-queue --queue-name trade_imports_inbound_customs_declarations_processor-deadletter.fifo --attributes FifoQueue=true
awslocal sqs create-queue --queue-name trade_imports_inbound_customs_declarations_processor.fifo --attributes '{"FifoQueue":"true","RedrivePolicy": "{\"deadLetterTargetArn\":\"arn:aws:sqs:eu-west-2:000000000000:trade_gateway_publisher_ched_updates_processor-deadletter.fifo\",\"maxReceiveCount\":\"1\"}"}'
    
awslocal sqs create-queue --queue-name trade_imports_data_upserted_processor-deadletter
awslocal sqs create-queue --queue-name trade_imports_data_upserted_processor --attributes '{"RedrivePolicy": "{\"deadLetterTargetArn\":\"arn:aws:sqs:eu-west-2:000000000000:trade_imports_data_upserted_processor-deadletter\",\"maxReceiveCount\":\"1\"}"}'

awslocal sqs create-queue --queue-name trade_imports_matched_gmrs_btms_processor-deadletter
awslocal sqs create-queue --queue-name trade_imports_matched_gmrs_btms_processor --attributes '{"RedrivePolicy": "{\"deadLetterTargetArn\":\"arn:aws:sqs:eu-west-2:000000000000:trade_imports_matched_gmrs_btms_processor-deadletter\",\"maxReceiveCount\":\"1\"}"}'
