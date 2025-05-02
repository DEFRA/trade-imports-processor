using System.Text.Json;
using Defra.TradeImportsProcessor.Processor.Models.ImportNotification;
using DataApiIpaffs = Defra.TradeImportsDataApi.Domain.Ipaffs;

namespace Defra.TradeImportsProcessor.Processor.Tests.Models;

public class ImportNotificationTests
{
    private const string ImportNotificationFixture = """
        {
            "id": 4353670,
            "referenceNumber": "CHEDD.GB.2024.5555554",
            "version": 2,
            "lastUpdated": "2024-12-28T19:45:21.131971885Z",
            "lastUpdatedBy": {
                "displayName": "40cfe371811e66d3e37dd709c7151f43734a994cbc2919ba22edd51a5c7ce7cb",
                "userId": "445f233c-c29c-ed11-aad1-000d3aad6e61"
            },
            "type": "CED",
            "status": "VALIDATED",
            "isHighRiskEuImport": false,
            "partOne": {
                "personResponsible": {
                    "name": "56beb672f5630cbe67a2c7727671a49aa43cff102ad1f5dee44904c51d2e86a4",
                    "companyId": "c952f246-f13f-e911-a95d-000d3a454f67",
                    "companyName": "30a3b779fcca46e08121431624153c672bcd53a3eb2ae35c35bab9389f97e662",
                    "address": [
                        "3e96ffa3fb973e41c58179889f5cacb3fc49ae94a2486cd786d830c8666e7d79",
                        "532f3d0f81eefee7a1045b18ab5da0459aad599c2dcd4d8a4b26b5200a8fa682",
                        "3e613288cbc226922f9ed033f875c32a40911c9e57907bd343890b5a0767d8a2",
                        "ca0116e9796a0ba36971994101af9c536b1a2c63b0f10fbd971b856936a44c37"
                    ],
                    "country": "GB",
                    "tracesID": 1001,
                    "phone": "d93fa153f66c3ff8000e98675833290bf72ee473fa62f846abd412016223f60f",
                    "email": "7d11ee06651612de5533870ec1d5a93d19b130b7c81192308d32bb4d8c61fb83",
                    "contactId": "4019cd89-3ae7-ec11-bb3c-00224881a955"
                },
                "consignor": {
                    "id": "e436e866-7001-4bc0-9e29-95549e29dcbc",
                    "type": "exporter",
                    "status": "nonapproved",
                    "companyName": "0c37ea33df2de8754c9912a8cdaf9e6d8784ea63c39933e0d06675c600c2922a",
                    "address": {
                        "addressLine1": "6c0b94972fb2b969e7468d961f39d35a11651a0d4d23bd25be3b56ff597c6048",
                        "addressLine2": "815aab766f0e83bca1271386420e58850f2806e9882bac63243f5b732fbf2d3b",
                        "addressLine3": "182ae1b4584be01991223f509fb1b1593eea63a2e9f83e721498bd8839cd05db",
                        "city": "5bfc2e5513a487037e6abb8404385b6e0a5a1b6a05d038588f9efef0a46c85f1",
                        "postalZipCode": "01b3ee1213dc2b3b0145f1ff45ef1701db3a53e54f9dada2042501ddf2739c12",
                        "countryISOCode": "KE",
                        "telephone": "9d8d095cce96670f75bb0be5dd6afaa25c6f89ac4a5c8ff91a4b78dece9d2e68",
                        "email": "860853b4fe1c588a5adf0aad03015a295daaa39e083e14d7e0c1d133aa6a45b8"
                    },
                    "tracesId": 10003819
                },
                "consignee": {
                    "id": "c447aa44-5a57-4ca7-8f28-f87eb7403517",
                    "type": "consignee",
                    "status": "nonapproved",
                    "companyName": "8c4eff55bbc5e179632b60ec8df89bf9d172742f11ff5b755ad8459e4257d195",
                    "address": {
                        "addressLine1": "e9444f8f9beb2320fbbc07f89734cf25ee7334b803083cb9e3c6eef3188cf220",
                        "addressLine2": "3970bf92db970a7df201ab0136b2715716d3a1587d8fe138ba2394104f79c756",
                        "city": "ecc0e7dc084f141b29479058967d0bc07dee25d9690a98ee4e6fdad5168274d7",
                        "postalZipCode": "8719e2971acea934088f8541cca26cdff37a2bce85b3e9dec6192470a282f08f",
                        "countryISOCode": "GB-ENG",
                        "ukTelephone": "3eda99ab0eaabb30b311900f8a83efc725fb4702788904dcee9b3feb18c28e20",
                        "telephone": "3eda99ab0eaabb30b311900f8a83efc725fb4702788904dcee9b3feb18c28e20",
                        "email": "83387ed2fd5327925348770604b03ed3c99d2be8ba0d692764f6b31f7f7b1c8e"
                    },
                    "tracesId": 10003487
                },
                "importer": {
                    "id": "c447aa44-5a57-4ca7-8f28-f87eb7403517",
                    "type": "consignee",
                    "status": "nonapproved",
                    "companyName": "8c4eff55bbc5e179632b60ec8df89bf9d172742f11ff5b755ad8459e4257d195",
                    "address": {
                        "addressLine1": "e9444f8f9beb2320fbbc07f89734cf25ee7334b803083cb9e3c6eef3188cf220",
                        "addressLine2": "3970bf92db970a7df201ab0136b2715716d3a1587d8fe138ba2394104f79c756",
                        "city": "ecc0e7dc084f141b29479058967d0bc07dee25d9690a98ee4e6fdad5168274d7",
                        "postalZipCode": "8719e2971acea934088f8541cca26cdff37a2bce85b3e9dec6192470a282f08f",
                        "countryISOCode": "GB-ENG",
                        "ukTelephone": "3eda99ab0eaabb30b311900f8a83efc725fb4702788904dcee9b3feb18c28e20",
                        "telephone": "3eda99ab0eaabb30b311900f8a83efc725fb4702788904dcee9b3feb18c28e20",
                        "email": "83387ed2fd5327925348770604b03ed3c99d2be8ba0d692764f6b31f7f7b1c8e"
                    },
                    "tracesId": 10003487
                },
                "placeOfDestination": {
                    "id": "c447aa44-5a57-4ca7-8f28-f87eb7403517",
                    "type": "consignee",
                    "status": "nonapproved",
                    "companyName": "8c4eff55bbc5e179632b60ec8df89bf9d172742f11ff5b755ad8459e4257d195",
                    "address": {
                        "addressLine1": "e9444f8f9beb2320fbbc07f89734cf25ee7334b803083cb9e3c6eef3188cf220",
                        "addressLine2": "3970bf92db970a7df201ab0136b2715716d3a1587d8fe138ba2394104f79c756",
                        "city": "ecc0e7dc084f141b29479058967d0bc07dee25d9690a98ee4e6fdad5168274d7",
                        "postalZipCode": "8719e2971acea934088f8541cca26cdff37a2bce85b3e9dec6192470a282f08f",
                        "countryISOCode": "GB-ENG",
                        "ukTelephone": "3eda99ab0eaabb30b311900f8a83efc725fb4702788904dcee9b3feb18c28e20",
                        "telephone": "3eda99ab0eaabb30b311900f8a83efc725fb4702788904dcee9b3feb18c28e20",
                        "email": "83387ed2fd5327925348770604b03ed3c99d2be8ba0d692764f6b31f7f7b1c8e"
                    },
                    "tracesId": 10003487
                },
                "commodities": {
                    "totalGrossWeight": 2554,
                    "totalNetWeight": 1008,
                    "numberOfPackages": 19,
                    "temperature": "Chilled",
                    "commodityComplement": [
                        {
                            "commodityID": "070820",
                            "commodityDescription": "Beans (Vigna spp., Phaseolus spp.)",
                            "complementID": 2,
                            "speciesType": "070820",
                            "speciesClass": "070820"
                        }
                    ],
                    "complementParameterSet": [
                        {
                            "uniqueComplementID": "1bd0e706-705f-4d86-add7-dc58441e98d3",
                            "complementID": 2,
                            "keyDataPair": [
                                {
                                    "key": "netweight",
                                    "data": "1008"
                                },
                                {
                                    "key": "number_package",
                                    "data": "19"
                                },
                                {
                                    "key": "type_package",
                                    "data": "Box"
                                }
                            ]
                        }
                    ],
                    "includeNonAblactedAnimals": false,
                    "countryOfOrigin": "KE",
                    "isLowRiskArticle72Country": false,
                    "consignedCountry": "KE",
                    "commodityIntendedFor": "human"
                },
                "purpose": {
                    "purposeGroup": "For Import"
                },
                "pointOfEntry": "GBLHR4P",
                "arrivalDate": "2024-12-28",
                "arrivalTime": "19:24:00",
                "transporterDetailsRequired": false,
                "meansOfTransport": {},
                "meansOfTransportFromEntryPoint": {
                    "id": "KQ100",
                    "type": "Aeroplane",
                    "document": "701 5123 8493"
                },
                "veterinaryInformation": {
                    "accompanyingDocuments": [
                        {
                            "documentType": "commercialInvoice",
                            "documentReference": "087957",
                            "documentIssueDate": "2024-12-27"
                        }
                    ]
                },
                "importerLocalReferenceNumber": "HR/00/00/000",
                "submissionDate": "2024-12-28T00:16:22.582187163Z",
                "submittedBy": {
                    "displayName": "56beb672f5630cbe67a2c7727671a49aa43cff102ad1f5dee44904c51d2e86a4",
                    "userId": "4019cd89-3ae7-ec11-bb3c-00224881a955"
                },
                "complexCommoditySelected": true,
                "portOfEntry": "GBLHRE",
                "contactDetails": {
                    "name": "Dave",
                    "telephone": "01211 121333",
                    "email": "dave@theimporter"
                },
                "isGVMSRoute": false,
                "provideCtcMrn": "NO"
            },
            "decisionBy": {
                "displayName": "40cfe371811e66d3e37dd709c7151f43734a994cbc2919ba22edd51a5c7ce7cb",
                "userId": "111f233c-c29c-ed11-aad1-000d3aad6e61"
            },
            "decisionDate": "2024-12-28T19:45:21.104623034Z",
            "partTwo": {
                "decision": {
                    "consignmentAcceptable": true,
                    "decision": "Acceptable for Internal Market",
                    "freeCirculationPurpose": "Human Consumption"
                },
                "consignmentCheck": {
                    "documentCheckResult": "Satisfactory",
                    "identityCheckDone": false,
                    "physicalCheckDone": false
                },
                "laboratoryTestsRequired": false,
                "resealedContainersIncluded": false,
                "controlAuthority": {
                    "officialVeterinarian": {
                        "firstName": "e054bd687e31b4c25474e508130c6e8ae2e8ad86916dc115072ed05ad867d332",
                        "lastName": "fa30ecf572b9cae646e2531ed9da518400e884d5ea936c72a48478ec01713e4c",
                        "email": "3d1607538dd44d6ee71cf17ca74f211e2213bf99c64932fa5e3fcbc38a4a6d27",
                        "phone": "5d27d17535b78cb08f56fd475a10fc73660bee09e93f34b62238160bc74627e9",
                        "signed": "2024-12-28T19:45:21.131940384"
                    }
                },
                "bipLocalReferenceNumber": "HR/11/11/11",
                "checkDate": "2024-12-28T19:45:00Z"
            },
            "partThree": {
                "sealCheckRequired": false
            },
            "etag": "0000000004790B83",
            "riskDecisionLockingTime": "2024-12-28T17:24:00Z",
            "isRiskDecisionLocked": false,
            "chedTypeVersion": 2
        }
        """;

    [Fact]
    public Task ImportNotification_DeserializedFromASB_IsCorrect()
    {
        var importNotification = JsonSerializer.Deserialize<ImportNotification>(ImportNotificationFixture);
        Assert.NotNull(importNotification);

        return Verify(importNotification).DontScrubDateTimes();
    }

    [Fact]
    public Task ImportNotification_ConversionToDataApiImportPreNotification_IsCorrect()
    {
        var importNotification = JsonSerializer.Deserialize<ImportNotification>(ImportNotificationFixture);
        Assert.NotNull(importNotification);

        var dataApiImportPreNotification = (DataApiIpaffs.ImportPreNotification)importNotification;
        return Verify(dataApiImportPreNotification).DontScrubDateTimes();
    }
}
