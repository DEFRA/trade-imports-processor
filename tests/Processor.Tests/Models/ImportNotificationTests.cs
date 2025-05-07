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
                    "purposeGroup": "For Import",
                    "pointOfExit": "BCP1"
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
                    "physicalCheckDone": false,
                    "documentCheckAdditionalDetails": "Other things"
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
            "isAutoClearanceExempted": false,
            "chedTypeVersion": 2
        }
        """;

    private const string ImportNotificationWithRiskAssessmentFixture = """
        {
          "id": 4194492,
          "referenceNumber": "CHEDPP.GB.2024.5194492",
          "agencyOrganisationId": "b65caccc-a1ea-e911-a812-000d3a4aaef5",
          "version": 1,
          "lastUpdated": "2024-11-24T06:22:34.924396145Z",
          "lastUpdatedBy": {
            "displayName": "b960cda9180fe306ec4691edce6ed6bbbe26424670e77078ac31f889fa5ea419",
            "userId": "3dd58971-2d3f-e911-a95d-000d3a454f67"
          },
          "type": "CHEDPP",
          "status": "IN_PROGRESS",
          "riskAssessment": {
            "commodityResults": [
              {
                "hmiDecision": "NOTREQUIRED",
                "phsiDecision": "REQUIRED",
                "phsiClassification": "Mandatory",
                "phsi": {
                  "documentCheck": true,
                  "identityCheck": true,
                  "physicalCheck": true
                },
                "uniqueId": "d6437e44-042d-4aad-b8b1-c8cbcceab091",
                "eppoCode": "PELSS",
                "isWoody": true,
                "indoorOutdoor": "Outdoor",
                "propagation": "Plant",
                "phsiRuleType": "Consignment"
              },
              {
                "hmiDecision": "NOTREQUIRED",
                "phsiDecision": "REQUIRED",
                "phsiClassification": "Reduced",
                "phsi": {
                  "documentCheck": true,
                  "identityCheck": true,
                  "physicalCheck": true
                },
                "uniqueId": "b2a8be6b-0170-4385-9a9d-eb0a6ffbf7d2",
                "eppoCode": "CHYFR",
                "isWoody": false,
                "indoorOutdoor": "Outdoor",
                "propagation": "Plant",
                "phsiRuleType": "Consignment"
              },
              {
                "hmiDecision": "NOTREQUIRED",
                "phsiDecision": "REQUIRED",
                "phsiClassification": "Reduced",
                "phsi": {
                  "documentCheck": true,
                  "identityCheck": true,
                  "physicalCheck": true
                },
                "uniqueId": "d90eaefc-9caa-47d6-9395-474869ea97d6",
                "eppoCode": "BIDFE",
                "isWoody": false,
                "indoorOutdoor": "Outdoor",
                "propagation": "Plant",
                "phsiRuleType": "Consignment"
              },
              {
                "hmiDecision": "NOTREQUIRED",
                "phsiDecision": "REQUIRED",
                "phsiClassification": "Reduced",
                "phsi": {
                  "documentCheck": true,
                  "identityCheck": true,
                  "physicalCheck": true
                },
                "uniqueId": "d43a159a-4ab2-419c-a15b-e3b8e2bcd943",
                "eppoCode": "FUCSS",
                "isWoody": false,
                "indoorOutdoor": "Outdoor",
                "propagation": "Plant",
                "phsiRuleType": "Consignment"
              },
              {
                "hmiDecision": "NOTREQUIRED",
                "phsiDecision": "REQUIRED",
                "phsiClassification": "Mandatory",
                "phsi": {
                  "documentCheck": true,
                  "identityCheck": true,
                  "physicalCheck": true
                },
                "uniqueId": "4cb5f658-a9e6-4662-8775-8c194a3b659d",
                "eppoCode": "GAASS",
                "isWoody": true,
                "indoorOutdoor": "Outdoor",
                "propagation": "Plant",
                "phsiRuleType": "Consignment"
              },
              {
                "hmiDecision": "NOTREQUIRED",
                "phsiDecision": "REQUIRED",
                "phsiClassification": "Reduced",
                "phsi": {
                  "documentCheck": true,
                  "identityCheck": true,
                  "physicalCheck": true
                },
                "uniqueId": "6fd1e381-e2d4-4506-bb4b-9a7d6c370396",
                "eppoCode": "GLESS",
                "isWoody": false,
                "indoorOutdoor": "Outdoor",
                "propagation": "Plant",
                "phsiRuleType": "Consignment"
              }
            ],
            "assessmentDateTime": "2024-11-23T22:06:05.266035044"
          },
          "partOne": {
            "personResponsible": {
              "name": "c91b1e77f4d64ef1fb982524a786483f5be4d3393664b8abccd61c7ed81330fa",
              "companyId": "590d9016-2946-ec11-8c62-6045bd8db76d",
              "companyName": "419c4d2874684c58c2429fbc281a7accdb63b447703b98249c73ec48a545ca4e",
              "address": [
                "bf4227af78e742aca0eddd71616de5ccd04c3ec85547d194355fb23b9f252971",
                "ef2d2183722cc5ea358ca0c281f4714d5f3d01be51c274024be7ba950bffe938",
                "cb29b5b5cb165e24a262eb5a5fcd7d1f910e2ea8354827c0b17becbf7f1f9f84",
                "47fe17ce1714143d2c3e39d3000760d4be7072e61f229f46a47986adfe2d2f27",
                "65c0575c76b78ef9e6c46ff0bf0ddc1622f9f5644bbca3c73ca01c202df6d91e",
                "b4043b0b8297e379bc559ab33b6ae9c7a9b4ef6519d3baee53270f0c0dd3d960"
              ],
              "country": "GB",
              "tracesID": 1001,
              "phone": "f3c2d053fee804925a9c4925166edf92b0dd3bc6f3408baf27296e19b526bca3",
              "email": "748494e51b3a382532618f495f601f196df797d8b9b44eb70ef93bcfedd86422",
              "contactId": "470d9016-2946-ec11-8c62-6045bd8db76d",
              "storeTransporterContact": "NO"
            },
            "consignor": {
              "id": "02ed887e-bf0c-4d9c-af7a-738c50916cc8",
              "type": "exporter",
              "status": "nonapproved",
              "companyName": "a6c61915e8228beb7841d2c997cef5b2dc24fc657fedd2456aa8cf212603dc4f",
              "address": {
                "addressLine1": "6a3bd35fb22e108b1e520c2c4e3ace7043f57afea30af0cec581beef0680a1ff",
                "city": "2b90c13170e642ff6a0f553e88532d891399326a335a543af9b4ddce094fd05e",
                "postalZipCode": "9203419f3d0364844a36b254c667181967255460116fe20c0a51b06f98ac4788",
                "countryISOCode": "KE",
                "telephone": "0b8d7184f6f6ae75f64caab27497bf711267d17323e362ee854bc6ddabb5cb92",
                "email": "b02c7f508eb7ac55b9f9c27a6e7d77d22c1cae8ce5d6b37b267a3a660d9dc953"
              },
              "tracesId": 10221705
            },
            "consignee": {
              "id": "590d9016-2946-ec11-8c62-6045bd8db76d",
              "type": "consignee",
              "companyName": "419c4d2874684c58c2429fbc281a7accdb63b447703b98249c73ec48a545ca4e",
              "address": {
                "addressLine1": "bf4227af78e742aca0eddd71616de5ccd04c3ec85547d194355fb23b9f252971",
                "addressLine2": "ef2d2183722cc5ea358ca0c281f4714d5f3d01be51c274024be7ba950bffe938",
                "city": "47fe17ce1714143d2c3e39d3000760d4be7072e61f229f46a47986adfe2d2f27",
                "postalZipCode": "65c0575c76b78ef9e6c46ff0bf0ddc1622f9f5644bbca3c73ca01c202df6d91e",
                "countryISOCode": "GB",
                "telephone": "f3c2d053fee804925a9c4925166edf92b0dd3bc6f3408baf27296e19b526bca3",
                "email": "748494e51b3a382532618f495f601f196df797d8b9b44eb70ef93bcfedd86422"
              },
              "tracesId": 10221732
            },
            "importer": {
              "id": "590d9016-2946-ec11-8c62-6045bd8db76d",
              "type": "importer",
              "companyName": "419c4d2874684c58c2429fbc281a7accdb63b447703b98249c73ec48a545ca4e",
              "address": {
                "addressLine1": "bf4227af78e742aca0eddd71616de5ccd04c3ec85547d194355fb23b9f252971",
                "addressLine2": "ef2d2183722cc5ea358ca0c281f4714d5f3d01be51c274024be7ba950bffe938",
                "city": "47fe17ce1714143d2c3e39d3000760d4be7072e61f229f46a47986adfe2d2f27",
                "postalZipCode": "65c0575c76b78ef9e6c46ff0bf0ddc1622f9f5644bbca3c73ca01c202df6d91e",
                "countryISOCode": "GB",
                "telephone": "f3c2d053fee804925a9c4925166edf92b0dd3bc6f3408baf27296e19b526bca3",
                "email": "748494e51b3a382532618f495f601f196df797d8b9b44eb70ef93bcfedd86422"
              },
              "tracesId": 10221732
            },
            "placeOfDestination": {
              "id": "590d9016-2946-ec11-8c62-6045bd8db76d",
              "type": "destination",
              "companyName": "419c4d2874684c58c2429fbc281a7accdb63b447703b98249c73ec48a545ca4e",
              "address": {
                "addressLine1": "bf4227af78e742aca0eddd71616de5ccd04c3ec85547d194355fb23b9f252971",
                "addressLine2": "ef2d2183722cc5ea358ca0c281f4714d5f3d01be51c274024be7ba950bffe938",
                "city": "47fe17ce1714143d2c3e39d3000760d4be7072e61f229f46a47986adfe2d2f27",
                "postalZipCode": "65c0575c76b78ef9e6c46ff0bf0ddc1622f9f5644bbca3c73ca01c202df6d91e",
                "countryISOCode": "GB",
                "telephone": "f3c2d053fee804925a9c4925166edf92b0dd3bc6f3408baf27296e19b526bca3",
                "email": "748494e51b3a382532618f495f601f196df797d8b9b44eb70ef93bcfedd86422"
              },
              "tracesId": 10221732
            },
            "commodities": {
              "totalGrossWeight": 65,
              "totalNetWeight": 59.5,
              "numberOfPackages": 17,
              "commodityComplement": [
                {
                  "commodityID": "06021090",
                  "commodityDescription": "Other",
                  "complementID": 3,
                  "complementName": "Pelargonium sp.",
                  "eppoCode": "PELSS",
                  "speciesID": "1345841",
                  "speciesName": "Pelargonium sp.",
                  "speciesNomination": "Pelargonium sp."
                },
                {
                  "commodityID": "06021090",
                  "commodityDescription": "Other",
                  "complementID": 4,
                  "complementName": "Argyranthemum frutescens",
                  "eppoCode": "CHYFR",
                  "speciesID": "1364198",
                  "speciesName": "Argyranthemum frutescens",
                  "speciesNomination": "Argyranthemum frutescens"
                },
                {
                  "commodityID": "06021090",
                  "commodityDescription": "Other",
                  "complementID": 5,
                  "complementName": "Bidens ferulifolia",
                  "eppoCode": "BIDFE",
                  "speciesID": "1402711",
                  "speciesName": "Bidens ferulifolia",
                  "speciesNomination": "Bidens ferulifolia"
                },
                {
                  "commodityID": "06021090",
                  "commodityDescription": "Other",
                  "complementID": 6,
                  "complementName": "Fuchsia sp.",
                  "eppoCode": "FUCSS",
                  "speciesID": "1352563",
                  "speciesName": "Fuchsia sp.",
                  "speciesNomination": "Fuchsia sp."
                },
                {
                  "commodityID": "06021090",
                  "commodityDescription": "Other",
                  "complementID": 7,
                  "complementName": "Gaura sp.",
                  "eppoCode": "GAASS",
                  "speciesID": "1443326",
                  "speciesName": "Gaura sp.",
                  "speciesNomination": "Gaura sp."
                },
                {
                  "commodityID": "06021090",
                  "commodityDescription": "Other",
                  "complementID": 8,
                  "complementName": "Glechoma sp.",
                  "eppoCode": "GLESS",
                  "speciesID": "1365075",
                  "speciesName": "Glechoma sp.",
                  "speciesNomination": "Glechoma sp."
                }
              ],
              "complementParameterSet": [
                {
                  "uniqueComplementID": "d6437e44-042d-4aad-b8b1-c8cbcceab091",
                  "complementID": 3,
                  "speciesID": "1345841",
                  "keyDataPair": [
                    {
                      "key": "regulatory_authority",
                      "data": "PHSI"
                    },
                    {
                      "key": "requires_finished_or_propagated_data",
                      "data": "true"
                    },
                    {
                      "key": "propagation",
                      "data": "Plant"
                    },
                    {
                      "key": "type_package",
                      "data": "Carton"
                    },
                    {
                      "key": "commodity_group",
                      "data": "Plants for Planting"
                    },
                    {
                      "key": "netweight",
                      "data": "42"
                    },
                    {
                      "key": "number_package",
                      "data": "12"
                    },
                    {
                      "key": "quantity",
                      "data": "22700"
                    },
                    {
                      "key": "type_quantity",
                      "data": "Pieces"
                    },
                    {
                      "key": "finished_or_propagated",
                      "data": "Propagated"
                    }
                  ]
                },
                {
                  "uniqueComplementID": "b2a8be6b-0170-4385-9a9d-eb0a6ffbf7d2",
                  "complementID": 4,
                  "speciesID": "1364198",
                  "keyDataPair": [
                    {
                      "key": "regulatory_authority",
                      "data": "PHSI"
                    },
                    {
                      "key": "requires_finished_or_propagated_data",
                      "data": "true"
                    },
                    {
                      "key": "propagation",
                      "data": "Plant"
                    },
                    {
                      "key": "type_package",
                      "data": "Carton"
                    },
                    {
                      "key": "netweight",
                      "data": "3.5"
                    },
                    {
                      "key": "number_package",
                      "data": "1"
                    },
                    {
                      "key": "quantity",
                      "data": "1400"
                    },
                    {
                      "key": "type_quantity",
                      "data": "Pieces"
                    },
                    {
                      "key": "commodity_group",
                      "data": "Plants for Planting"
                    },
                    {
                      "key": "finished_or_propagated",
                      "data": "Propagated"
                    }
                  ]
                },
                {
                  "uniqueComplementID": "d90eaefc-9caa-47d6-9395-474869ea97d6",
                  "complementID": 5,
                  "speciesID": "1402711",
                  "keyDataPair": [
                    {
                      "key": "regulatory_authority",
                      "data": "PHSI"
                    },
                    {
                      "key": "requires_finished_or_propagated_data",
                      "data": "true"
                    },
                    {
                      "key": "propagation",
                      "data": "Plant"
                    },
                    {
                      "key": "type_package",
                      "data": "Carton"
                    },
                    {
                      "key": "netweight",
                      "data": "3.5"
                    },
                    {
                      "key": "number_package",
                      "data": "1"
                    },
                    {
                      "key": "quantity",
                      "data": "1500"
                    },
                    {
                      "key": "type_quantity",
                      "data": "Pieces"
                    },
                    {
                      "key": "commodity_group",
                      "data": "Plants for Planting"
                    },
                    {
                      "key": "finished_or_propagated",
                      "data": "Propagated"
                    }
                  ]
                },
                {
                  "uniqueComplementID": "d43a159a-4ab2-419c-a15b-e3b8e2bcd943",
                  "complementID": 6,
                  "speciesID": "1352563",
                  "keyDataPair": [
                    {
                      "key": "regulatory_authority",
                      "data": "PHSI"
                    },
                    {
                      "key": "requires_finished_or_propagated_data",
                      "data": "true"
                    },
                    {
                      "key": "propagation",
                      "data": "Plant"
                    },
                    {
                      "key": "type_package",
                      "data": "Carton"
                    },
                    {
                      "key": "netweight",
                      "data": "3.5"
                    },
                    {
                      "key": "number_package",
                      "data": "1"
                    },
                    {
                      "key": "quantity",
                      "data": "4300"
                    },
                    {
                      "key": "type_quantity",
                      "data": "Pieces"
                    },
                    {
                      "key": "commodity_group",
                      "data": "Plants for Planting"
                    },
                    {
                      "key": "finished_or_propagated",
                      "data": "Propagated"
                    }
                  ]
                },
                {
                  "uniqueComplementID": "4cb5f658-a9e6-4662-8775-8c194a3b659d",
                  "complementID": 7,
                  "speciesID": "1443326",
                  "keyDataPair": [
                    {
                      "key": "regulatory_authority",
                      "data": "PHSI"
                    },
                    {
                      "key": "requires_finished_or_propagated_data",
                      "data": "true"
                    },
                    {
                      "key": "propagation",
                      "data": "Plant"
                    },
                    {
                      "key": "type_package",
                      "data": "Carton"
                    },
                    {
                      "key": "netweight",
                      "data": "3.5"
                    },
                    {
                      "key": "number_package",
                      "data": "1"
                    },
                    {
                      "key": "quantity",
                      "data": "2600"
                    },
                    {
                      "key": "type_quantity",
                      "data": "Pieces"
                    },
                    {
                      "key": "commodity_group",
                      "data": "Plants for Planting"
                    },
                    {
                      "key": "finished_or_propagated",
                      "data": "Propagated"
                    }
                  ]
                },
                {
                  "uniqueComplementID": "6fd1e381-e2d4-4506-bb4b-9a7d6c370396",
                  "complementID": 8,
                  "speciesID": "1365075",
                  "keyDataPair": [
                    {
                      "key": "regulatory_authority",
                      "data": "PHSI"
                    },
                    {
                      "key": "requires_finished_or_propagated_data",
                      "data": "true"
                    },
                    {
                      "key": "propagation",
                      "data": "Plant"
                    },
                    {
                      "key": "type_package",
                      "data": "Carton"
                    },
                    {
                      "key": "netweight",
                      "data": "3.5"
                    },
                    {
                      "key": "number_package",
                      "data": "1"
                    },
                    {
                      "key": "quantity",
                      "data": "600"
                    },
                    {
                      "key": "type_quantity",
                      "data": "Pieces"
                    },
                    {
                      "key": "commodity_group",
                      "data": "Plants for Planting"
                    },
                    {
                      "key": "finished_or_propagated",
                      "data": "Propagated"
                    }
                  ]
                }
              ],
              "includeNonAblactedAnimals": false,
              "countryOfOrigin": "KE",
              "countryOfOriginIsPodCountry": false,
              "consignedCountry": "KE",
              "consignedCountryInChargeGroup": false
            },
            "purpose": {
              "purposeGroup": "For Import",
              "pointOfExit": "BCP1"
            },
            "pointOfEntry": "CONPNT",
            "pointOfEntryControlPoint": "IPGBNHR6",
            "arrivalDate": "2024-11-24",
            "arrivalTime": "14:00:00",
            "meansOfTransport": {},
            "meansOfTransportFromEntryPoint": {
              "id": "BA064",
              "type": "Aeroplane",
              "document": "125-14531123"
            },
            "veterinaryInformation": {
              "accompanyingDocuments": [
                {
                  "documentType": "phytosanitaryCertificate",
                  "documentReference": "KEPHIS/EXP/2024/1111111",
                  "documentIssueDate": "2024-11-22",
                  "attachmentId": "ffcba8d7-09e6-4ebb-b2ab-736190a7f388",
                  "attachmentFilename": "1877862.pdf",
                  "attachmentContentType": "application/pdf",
                  "uploadUserId": "405c9eda-edc0-ed11-83ff-000d3abd8f0e",
                  "uploadOrganisationId": "b65caccc-a1ea-e911-a812-000d3a4aaef5"
                },
                {
                  "documentType": "airWaybill",
                  "documentReference": "125-11111111",
                  "documentIssueDate": "2024-11-23",
                  "attachmentId": "58f247a2-fa95-40ef-9057-e6474f61ec83",
                  "attachmentFilename": "125-14531123.pdf",
                  "attachmentContentType": "application/pdf",
                  "uploadUserId": "405c9eda-edc0-ed11-83ff-000d3abd8f0e",
                  "uploadOrganisationId": "b65caccc-a1ea-e911-a812-000d3a4aaef5"
                }
              ]
            },
            "importerLocalReferenceNumber": "111111",
            "submissionDate": "2024-11-23T22:06:04.089939699Z",
            "submittedBy": {
              "displayName": "278eb2582153c57163668257c27e09f4b2d7b6a330c90668b2013c23024d668f",
              "userId": "405c9eda-edc0-ed11-83ff-000d3abd8f0e"
            },
            "complexCommoditySelected": true,
            "contactDetails": {
              "name": "Dave",
              "telephone": "01111111111",
              "email": "dave@theimporter",
              "agent": "Dave UK"
            },
            "isGVMSRoute": false,
            "provideCtcMrn": "NO"
          },
          "partTwo": {
            "controlAuthority": {
              "officialVeterinarian": {
                "firstName": "84a4b19e19aa4e2a562ae0286b1e188ef4f4f9a98a92b8730d20a1e0f2882523",
                "lastName": "6562ae2f1cfb3d4f482f8185875eca296a85bfcd4dac3a09ba2162023395aac5",
                "email": "c19f9bb5a41dbf1c2576f967a2a8208c957f3188861dfc687bb0a4c5beaa71aa",
                "phone": "902322b102aeb478028f7680104536eb36a1e5f0468c21b58a743ed1f3636d08",
                "signed": "2024-11-24T06:22:34.924328645"
              }
            },
            "commodityChecks": [
              {
                "uniqueComplementId": "d6437e44-042d-4aad-b8b1-c8cbcceab091",
                "checks": [
                  {
                    "type": "PHSI_DOCUMENT",
                    "status": "Compliant"
                  },
                  {
                    "type": "PHSI_IDENTITY",
                    "status": "To do"
                  },
                  {
                    "type": "PHSI_PHYSICAL",
                    "status": "To do"
                  }
                ]
              },
              {
                "uniqueComplementId": "b2a8be6b-0170-4385-9a9d-eb0a6ffbf7d2",
                "checks": [
                  {
                    "type": "PHSI_DOCUMENT",
                    "status": "Compliant"
                  },
                  {
                    "type": "PHSI_IDENTITY",
                    "status": "To do"
                  },
                  {
                    "type": "PHSI_PHYSICAL",
                    "status": "To do"
                  }
                ]
              },
              {
                "uniqueComplementId": "d90eaefc-9caa-47d6-9395-474869ea97d6",
                "checks": [
                  {
                    "type": "PHSI_DOCUMENT",
                    "status": "Compliant"
                  },
                  {
                    "type": "PHSI_IDENTITY",
                    "status": "To do"
                  },
                  {
                    "type": "PHSI_PHYSICAL",
                    "status": "To do"
                  }
                ]
              },
              {
                "uniqueComplementId": "d43a159a-4ab2-419c-a15b-e3b8e2bcd943",
                "checks": [
                  {
                    "type": "PHSI_DOCUMENT",
                    "status": "Compliant"
                  },
                  {
                    "type": "PHSI_IDENTITY",
                    "status": "To do"
                  },
                  {
                    "type": "PHSI_PHYSICAL",
                    "status": "To do"
                  }
                ]
              },
              {
                "uniqueComplementId": "4cb5f658-a9e6-4662-8775-8c194a3b659d",
                "checks": [
                  {
                    "type": "PHSI_DOCUMENT",
                    "status": "Compliant"
                  },
                  {
                    "type": "PHSI_IDENTITY",
                    "status": "To do"
                  },
                  {
                    "type": "PHSI_PHYSICAL",
                    "status": "To do"
                  }
                ]
              },
              {
                "uniqueComplementId": "6fd1e381-e2d4-4506-bb4b-9a7d6c370396",
                "checks": [
                  {
                    "type": "PHSI_DOCUMENT",
                    "status": "Compliant"
                  },
                  {
                    "type": "PHSI_IDENTITY",
                    "status": "To do"
                  },
                  {
                    "type": "PHSI_PHYSICAL",
                    "status": "To do"
                  }
                ]
              }
            ],
            "inspectionRequired": "Required"
          },
          "partThree": {},
          "etag": "00000000043C1660",
          "riskDecisionLockingTime": "2024-11-24T12:00:00Z",
          "isRiskDecisionLocked": false,
          "chedTypeVersion": 1
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

    [Fact]
    public Task ImportNotificationWithRiskAssessment_DeserializedFromASB_IsCorrect()
    {
        var importNotification = JsonSerializer.Deserialize<ImportNotification>(
            ImportNotificationWithRiskAssessmentFixture
        );
        Assert.NotNull(importNotification);

        return Verify(importNotification).DontScrubDateTimes();
    }

    [Fact]
    public Task ImportNotificationWithRiskAssessment_ConversionToDataApiImportPreNotification_IsCorrect()
    {
        var importNotification = JsonSerializer.Deserialize<ImportNotification>(
            ImportNotificationWithRiskAssessmentFixture
        );
        Assert.NotNull(importNotification);

        var dataApiImportPreNotification = (DataApiIpaffs.ImportPreNotification)importNotification;
        return Verify(dataApiImportPreNotification).DontScrubDateTimes();
    }
}
