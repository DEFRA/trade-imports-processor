#!/bin/bash

function is_ready() {
    awslocal sqs get-queue-url --queue-name trade_gateway_publisher_ched_updates_processor.fifo || return 1
    awslocal sqs get-queue-url --queue-name trade_gateway_publisher_ched_updates_processor-deadletter.fifo || return 1
    awslocal sqs get-queue-url --queue-name trade_imports_inbound_customs_declarations_processor.fifo || return 1
    awslocal sqs get-queue-url --queue-name trade_imports_inbound_customs_declarations_processor-deadletter.fifo || return 1
    awslocal sqs get-queue-url --queue-name trade_imports_data_upserted_processor || return 1
    awslocal sqs get-queue-url --queue-name trade_imports_data_upserted_processor-deadletter || return 1
    awslocal sqs get-queue-url --queue-name trade_imports_matched_gmrs_btms_processor || return 1
    awslocal sqs get-queue-url --queue-name trade_imports_matched_gmrs_btms_processor-deadletter || return 1
    return 0
}

is_ready