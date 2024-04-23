using Microsoft.SemanticKernel;
using OpenAIExtensions.Tools;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OpenAIExtensions.Plugins.WebSearch
{
    public class DsbTrainSchedulePlugin : WebSearchPlugin
    {
        [KernelFunction, Description("Search dsb.dk, for train schedules and prices")]
        public async Task<string> Search(
            Kernel kernel,
            [Description("From station, Ex: Copenhagen")] string from,
            [Description("To station, Ex: Malmo")] string to
        )
        {
         
            //var model = new NetbutikSearch
            //{
            //    Criteria = new List<Criteria>
            //    {
            //        new Criteria
            //        {
            //            Direction = 1,
            //            DepartLocation = new Location
            //            {
            //                Name = "København H",
            //                UicNumber = 8600626,
            //                TransportAuthority = ""
            //            },
            //            ArriveLocation = new Location
            //            {
            //                Name = "Malmö C",
            //                UicNumber = 7400003,
            //                TransportAuthority = ""
            //            },
            //            SearchDate = "2024-04-21",
            //            SearchTime = "20:23",
            //            Type = 0,
            //            SearchType = 1,
            //            SeatReservations = 0,
            //            PassengersAdults = 1,
            //            PassengersAdultsDiscount = 0,
            //            PassengersChildren = 0,
            //            PassengersChildrenDiscount = 0,
            //            PassengersYoungsters = 0,
            //            PassengersYoungstersDiscount = 0,
            //            PassengersSeniors = 0,
            //            PassengersSeniorsDiscount = 0,
            //            PassengersWildcard = 0,
            //            PassengersWildcardDiscount = 0,
            //            LimitTrainOnly = false,
            //            RequestBabyCarriage = false,
            //            JourneyId = 0
            //        }
            //    }
            //};

            //try
            //{
            //    var httpClient = new HttpClient();

            //    var searchUrl = @$"https://www.dsb.dk/api/netbutik/search";
            //    var request = new HttpRequestMessage(HttpMethod.Post, searchUrl);
            //    request.Headers.Add("Adrum", "isAjax:true");
            //    request.Headers.Add("Security-Token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL2V4cGlyYXRpb24iOiJhY2Nlc3MiLCJuYmYiOjE3MTM3MjIwNTMsImV4cCI6MTcxMzcyMjA4MywiaWF0IjoxNzEzNzIyMDUzfQ.MfnIUU88D2OLZ1bz08cNmXIbJ9df_OlQ0b7ZOjVeiNQ");
            //    request.Headers.Accept
            //        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    request.Headers.Add("Cookie", "__RequestVerificationToken=qXSx2QMQEVZWRlXAe23w7IxysobaDiwbjw8lvmT7A8BMMl07GUAw228VmXIvxtrGN31EKw8Kusve2UN_Gor6R-J-s7I1; gig_canary=false; ASP.NET_SessionId=4rmjpm1fohgxb12pnn232rtx; gig_bootstrap_4_sgl-E5_0k4GxgwQJBsMWLQ=ciam_ver4; CookieInformationConsent=%7B%22website_uuid%22%3A%225b4cc57d-c620-4627-872d-cd2925977bad%22%2C%22timestamp%22%3A%222024-04-19T09%3A16%3A09.555Z%22%2C%22consent_url%22%3A%22https%3A%2F%2Fwww.dsb.dk%2F%22%2C%22consent_website%22%3A%22dsb.dk%22%2C%22consent_domain%22%3A%22www.dsb.dk%22%2C%22user_uid%22%3A%222a79888c-8460-4d24-9048-f56b2b43956f%22%2C%22consents_approved%22%3A%5B%22cookie_cat_necessary%22%2C%22cookie_cat_functional%22%2C%22cookie_cat_statistic%22%2C%22cookie_cat_marketing%22%2C%22cookie_cat_unclassified%22%5D%2C%22consents_denied%22%3A%5B%5D%2C%22user_agent%22%3A%22Mozilla%2F5.0%20%28Windows%20NT%2010.0%3B%20Win64%3B%20x64%29%20AppleWebKit%2F537.36%20%28KHTML%2C%20like%20Gecko%29%20Chrome%2F123.0.0.0%20Safari%2F537.36%22%7D; _gcl_au=1.1.828797741.1713518170; monsido=BC91713518169879; _air360_i=MTRkYTYzZmQwMjNkMDFiODllMjViMjZkMmY2ZGVjMDU%3D; _fbp=fb.1.1713518169636.1209623680; _hjSessionUser_2425842=eyJpZCI6IjI4NWQyMzY2LTc4YzctNWMwOS05YWQwLTRjYmM5YmEyYzk3MyIsImNyZWF0ZWQiOjE3MTM1MTgxNzAwNjUsImV4aXN0aW5nIjp0cnVlfQ==; FPID=FPID2.2.IT2%2FgCu8RB3asKMpR8pWVO%2BIIF4hvq%2BREfRSadbIW6k%3D.1713518170; FPAU=1.1.828797741.1713518170; ontame_id.1eb6=9a5652e4-1213-4224-8f69-e538a5f491af.1713520233.1.1713520233.1713520233.194c00a8-1ed8-48dd-9be8-cb220a539f18; MSopened=1a3bbc9f92ee8eb76016f16743f648bc5afa3714; MSopened.1a3bbc9f92ee8eb76016f16743f648bc5afa3714=true; _gid=GA1.2.717739975.1713695995; FPLC=qGRhBepE0Z%2FwF8vFEoYCwZf3RQEvXBhgavx0AQ%2F6aO62iTBIix6QDrszfzzJYPrmNG8w0q%2BltapdubUjUo77TzLhI%2FXefoFf63TIqRxXltLFZRTOneZ1zbW%2FDqnxXg%3D%3D; _clck=1fcx7xk%7C2%7Cfl4%7C0%7C1570; gig_canary_ver=15877-3-28562025; _hjSession_2425842=eyJpZCI6IjEzOThlYzA1LTI1NWQtNGE5NS1iNjk2LTgzM2IzNWQ2OTljOSIsImMiOjE3MTM3MjE4NzYyNjQsInMiOjAsInIiOjAsInNiIjowLCJzciI6MCwic2UiOjAsImZzIjowLCJzcCI6MX0=; QueueITAccepted-SDFrts345E-V3_dsbdk2023=EventId%3Ddsbdk2023%26QueueId%3D00000000-0000-0000-0000-000000000000%26RedirectType%3Dafterevent%26IssueTime%3D1713721875%26Hash%3D86e8a3ef4fe2cadaf440f23c641ccdefa3ec169dfdfac6ad0ed99daa626cc36c; _gat_UA-2882912-1=1; FPGSID=1.1713721875.1713722030.G-L67Q4CCVRN.upfGl0nm1AqreN-CgAyCJw; engagedSessionTime=166; ADRUM=s=1713722041760&r=https%3A%2F%2Fwww.dsb.dk%2F%3F0; _ga=GA1.2.567137998.1713518170; _air360_s=ZWFkMTMyM2ItMjQyYS00ZGEzLTgwMDQtYjViN2Q4MjY2YTViLTE3MTM3MjE4NzZ8MTcxMzcyMjA0Mi41NTM%3D; _uetsid=7dfeaa20ffcb11ee9925e330c692c412; _uetvid=75dda900fe2d11eeb5637fed9c62ee64; _clsk=e6yrzp%7C1713722043120%7C3%7C1%7Ci.clarity.ms%2Fcollect; _ga_L67Q4CCVRN=GS1.1.1713721874.6.1.1713722053.0.0.1080985973");
            //    request.Content = new StringContent(JsonSerializer.Serialize(model), 
            //        Encoding.UTF8, 
            //        "application/json");

            //    var result = httpClient.SendAsync(request).GetAwaiter().GetResult();

            //    var html = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            //    if (string.IsNullOrEmpty(html))
            //    {
            //        return "";
            //    }

            //    string plainText = GetPlainTextFromHtml(html.ToString());

            //    return plainText.Substring(0, ContentLenght);
            //}
            //catch (Exception ex)
            //{

                
            //}

            return @"[
    {
        ""direction"": 1,
        ""journeys"": [
            {
                ""isLightningPlus"": false,
                ""numberOfSeats"": 0,
                ""isSeatsOnly"": false,
                ""isSeatReservationRequired"": false,
                ""stretches"": [
                    {
                        ""id"": 10,
                        ""lineName"": ""Regional-tog 1138"",
                        ""transportType"": {
                            ""id"": 23,
                            ""transportTypeFamily"": 1,
                            ""iconCssClass"": ""svg-train""
                        },
                        ""departLocation"": ""København H"",
                        ""departureDate"": ""2024-04-21T21:30:00+02:00"",
                        ""departureLocationIsInTS"": false,
                        ""departZoneInfo"": {
                            ""zone"": 1,
                            ""regionCodeNumber"": 1,
                            ""regionPrefix"": ""dot"",
                            ""isZoneSpecified"": true,
                            ""displayInfo"": ""dot 1""
                        },
                        ""arriveLocation"": ""Malmö C"",
                        ""arrivalDate"": ""2024-04-21T22:10:00+02:00"",
                        ""arrivalLocationIsInTS"": false,
                        ""arriveZoneInfo"": {
                            ""zone"": 0,
                            ""regionCodeNumber"": 0,
                            ""regionPrefix"": ""unknown – no zone info"",
                            ""isZoneSpecified"": false,
                            ""displayInfo"": ""unknown – no zone info""
                        },
                        ""isToFromBornholm"": false,
                        ""status"": 0
                    }
                ],
                ""ticketInstances"": [
                    {
                        ""journeyId"": 1,
                        ""code"": ""dsb_1_billet_og_plads"",
                        ""ticketName"": ""DSB 1' billet"",
                        ""ticketNameInDanish"": ""DSB 1' billet"",
                        ""ticketClass"": 2,
                        ""sortIndex"": 12,
                        ""isAvailable"": false,
                        ""isMainProduct"": true,
                        ""isVisible"": false,
                        ""seatReservationPrice"": 0.0,
                        ""ticketPrice"": 0.0,
                        ""discountedPrice"": 0.0,
                        ""isSeatsSoldOut"": false,
                        ""isReservationRequired"": false,
                        ""hasTickets"": false,
                        ""hasSeatReservations"": false,
                        ""passengers"": [],
                        ""seatReservations"": 0,
                        ""seatingGroups"": [],
                        ""productNotifications"": [],
                        ""stretchInfo"": {
                            ""10"": 0
                        },
                        ""ticketDetail"": {
                            ""type"": 0,
                            ""immediatelyIsAvailable"": false
                        },
                        ""hasBabyCarriage"": false,
                        ""hasBabyCarriageUnavailable"": false,
                        ""hasBabyCarriageAny"": false,
                        ""isOrangePartly"": false
                    },
                    {
                        ""journeyId"": 2,
                        ""code"": ""orange"",
                        ""ticketName"": ""DSB Orange billet"",
                        ""ticketNameInDanish"": ""DSB Orange billet"",
                        ""ticketClass"": 1,
                        ""sortIndex"": 22,
                        ""isAvailable"": false,
                        ""isMainProduct"": true,
                        ""isVisible"": false,
                        ""seatReservationPrice"": 0.0,
                        ""ticketPrice"": 0.0,
                        ""discountedPrice"": 0.0,
                        ""isSeatsSoldOut"": false,
                        ""isReservationRequired"": false,
                        ""hasTickets"": false,
                        ""hasSeatReservations"": false,
                        ""passengers"": [],
                        ""seatReservations"": 0,
                        ""seatingGroups"": [],
                        ""productNotifications"": [],
                        ""stretchInfo"": {
                            ""10"": 0
                        },
                        ""ticketDetail"": {
                            ""type"": 0,
                            ""immediatelyIsAvailable"": false
                        },
                        ""hasBabyCarriage"": false,
                        ""hasBabyCarriageUnavailable"": false,
                        ""hasBabyCarriageAny"": false,
                        ""isOrangePartly"": false
                    },
                    {
                        ""journeyId"": 3,
                        ""code"": ""orange_fri"",
                        ""ticketName"": ""DSB Orange Fri billet"",
                        ""ticketNameInDanish"": ""DSB Orange Fri billet"",
                        ""ticketClass"": 1,
                        ""sortIndex"": 23,
                        ""isAvailable"": false,
                        ""isMainProduct"": true,
                        ""isVisible"": false,
                        ""seatReservationPrice"": 0.0,
                        ""ticketPrice"": 0.0,
                        ""discountedPrice"": 0.0,
                        ""isSeatsSoldOut"": false,
                        ""isReservationRequired"": false,
                        ""hasTickets"": false,
                        ""hasSeatReservations"": false,
                        ""passengers"": [],
                        ""seatReservations"": 0,
                        ""seatingGroups"": [],
                        ""productNotifications"": [],
                        ""stretchInfo"": {
                            ""10"": 0
                        },
                        ""ticketDetail"": {
                            ""type"": 0,
                            ""immediatelyIsAvailable"": false
                        },
                        ""hasBabyCarriage"": false,
                        ""hasBabyCarriageUnavailable"": false,
                        ""hasBabyCarriageAny"": false,
                        ""isOrangePartly"": false
                    },
                    {
                        ""journeyId"": 0,
                        ""code"": ""standard_billet_og_plads"",
                        ""ticketName"": ""Standard billet"",
                        ""ticketNameInDanish"": ""Standard billet"",
                        ""ticketClass"": 1,
                        ""sortIndex"": 24,
                        ""isAvailable"": false,
                        ""isMainProduct"": true,
                        ""isVisible"": false,
                        ""seatReservationPrice"": 0.0,
                        ""ticketPrice"": 0.0,
                        ""discountedPrice"": 0.0,
                        ""isSeatsSoldOut"": false,
                        ""isReservationRequired"": false,
                        ""hasTickets"": false,
                        ""hasSeatReservations"": false,
                        ""passengers"": [],
                        ""seatReservations"": 0,
                        ""seatingGroups"": [],
                        ""productNotifications"": [],
                        ""stretchInfo"": {
                            ""10"": 0
                        },
                        ""ticketDetail"": {
                            ""type"": 0,
                            ""immediatelyIsAvailable"": false
                        },
                        ""hasBabyCarriage"": false,
                        ""hasBabyCarriageUnavailable"": false,
                        ""hasBabyCarriageAny"": false,
                        ""isOrangePartly"": false
                    }
                ],
                ""himMessages"": [],
                ""id"": 1,
                ""searchId"": 1517898110,
                ""lowestPrice"": 0.0,
                ""numberShifts"": 0,
                ""arrivalDate"": ""2024-04-21T22:10:00+02:00"",
                ""arrivalZoneInfo"": {
                    ""zone"": 0,
                    ""regionCodeNumber"": 0,
                    ""regionPrefix"": ""unknown – no zone info"",
                    ""isZoneSpecified"": false,
                    ""displayInfo"": ""unknown – no zone info""
                },
                ""arrivalLocation"": ""Malmö C"",
                ""departureDate"": ""2024-04-21T21:30:00+02:00"",
                ""departureZoneInfo"": {
                    ""zone"": 1,
                    ""regionCodeNumber"": 1,
                    ""regionPrefix"": ""dot"",
                    ""isZoneSpecified"": true,
                    ""displayInfo"": ""dot 1""
                },
                ""departLocation"": ""København H"",
                ""status"": 0,
                ""journeyTime"": 40,
                ""ticketPrice"": 0.0,
                ""seatReservationPrice"": 0.0,
                ""isAvailable"": false,
                ""productNotifications"": [
                    {
                        ""heading"": ""Din valgte rejse kan ikke købes her"",
                        ""body"": ""<p><span>Da Sk&aring;netrafikken har overtaget str&aelig;kningen, kan billetten ikke k&oslash;bes i DSBs salgskanaler. For billetter fra Hovedstadsomr&aring;det til Sydsverige henviser vi til <a href=\""https://www.skanetrafiken.se/\"" target=\""_blank\"" rel=\""noopener\"">skanetrafiken.se</a></span></p>"",
                        ""location"": 2,
                        ""productCodes"": []
                    }
                ],
                ""hasAvailableOrangePartly"": false
            },
            {
                ""isLightningPlus"": false,
                ""numberOfSeats"": 0,
                ""isSeatsOnly"": false,
                ""isSeatReservationRequired"": false,
                ""stretches"": [
                    {
                        ""id"": 20,
                        ""lineName"": ""Regional-tog 1140"",
                        ""transportType"": {
                            ""id"": 23,
                            ""transportTypeFamily"": 1,
                            ""iconCssClass"": ""svg-train""
                        },
                        ""departLocation"": ""København H"",
                        ""departureDate"": ""2024-04-21T21:44:00+02:00"",
                        ""departureLocationIsInTS"": false,
                        ""departZoneInfo"": {
                            ""zone"": 1,
                            ""regionCodeNumber"": 1,
                            ""regionPrefix"": ""dot"",
                            ""isZoneSpecified"": true,
                            ""displayInfo"": ""dot 1""
                        },
                        ""arriveLocation"": ""Malmö C"",
                        ""arrivalDate"": ""2024-04-21T22:25:00+02:00"",
                        ""arrivalLocationIsInTS"": false,
                        ""arriveZoneInfo"": {
                            ""zone"": 0,
                            ""regionCodeNumber"": 0,
                            ""regionPrefix"": ""unknown – no zone info"",
                            ""isZoneSpecified"": false,
                            ""displayInfo"": ""unknown – no zone info""
                        },
                        ""isToFromBornholm"": false,
                        ""status"": 0
                    }
                ],
                ""ticketInstances"": [
                    {
                        ""journeyId"": 5,
                        ""code"": ""dsb_1_billet_og_plads"",
                        ""ticketName"": ""DSB 1' billet"",
                        ""ticketNameInDanish"": ""DSB 1' billet"",
                        ""ticketClass"": 2,
                        ""sortIndex"": 12,
                        ""isAvailable"": false,
                        ""isMainProduct"": true,
                        ""isVisible"": false,
                        ""seatReservationPrice"": 0.0,
                        ""ticketPrice"": 0.0,
                        ""discountedPrice"": 0.0,
                        ""isSeatsSoldOut"": false,
                        ""isReservationRequired"": false,
                        ""hasTickets"": false,
                        ""hasSeatReservations"": false,
                        ""passengers"": [],
                        ""seatReservations"": 0,
                        ""seatingGroups"": [],
                        ""productNotifications"": [],
                        ""stretchInfo"": {
                            ""20"": 0
                        },
                        ""ticketDetail"": {
                            ""type"": 0,
                            ""immediatelyIsAvailable"": false
                        },
                        ""hasBabyCarriage"": false,
                        ""hasBabyCarriageUnavailable"": false,
                        ""hasBabyCarriageAny"": false,
                        ""isOrangePartly"": false
                    },
                    {
                        ""journeyId"": 6,
                        ""code"": ""orange"",
                        ""ticketName"": ""DSB Orange billet"",
                        ""ticketNameInDanish"": ""DSB Orange billet"",
                        ""ticketClass"": 1,
                        ""sortIndex"": 22,
                        ""isAvailable"": false,
                        ""isMainProduct"": true,
                        ""isVisible"": false,
                        ""seatReservationPrice"": 0.0,
                        ""ticketPrice"": 0.0,
                        ""discountedPrice"": 0.0,
                        ""isSeatsSoldOut"": false,
                        ""isReservationRequired"": false,
                        ""hasTickets"": false,
                        ""hasSeatReservations"": false,
                        ""passengers"": [],
                        ""seatReservations"": 0,
                        ""seatingGroups"": [],
                        ""productNotifications"": [],
                        ""stretchInfo"": {
                            ""20"": 0
                        },
                        ""ticketDetail"": {
                            ""type"": 0,
                            ""immediatelyIsAvailable"": false
                        },
                        ""hasBabyCarriage"": false,
                        ""hasBabyCarriageUnavailable"": false,
                        ""hasBabyCarriageAny"": false,
                        ""isOrangePartly"": false
                    },
                    {
                        ""journeyId"": 7,
                        ""code"": ""orange_fri"",
                        ""ticketName"": ""DSB Orange Fri billet"",
                        ""ticketNameInDanish"": ""DSB Orange Fri billet"",
                        ""ticketClass"": 1,
                        ""sortIndex"": 23,
                        ""isAvailable"": false,
                        ""isMainProduct"": true,
                        ""isVisible"": false,
                        ""seatReservationPrice"": 0.0,
                        ""ticketPrice"": 0.0,
                        ""discountedPrice"": 0.0,
                        ""isSeatsSoldOut"": false,
                        ""isReservationRequired"": false,
                        ""hasTickets"": false,
                        ""hasSeatReservations"": false,
                        ""passengers"": [],
                        ""seatReservations"": 0,
                        ""seatingGroups"": [],
                        ""productNotifications"": [],
                        ""stretchInfo"": {
                            ""20"": 0
                        },
                        ""ticketDetail"": {
                            ""type"": 0,
                            ""immediatelyIsAvailable"": false
                        },
                        ""hasBabyCarriage"": false,
                        ""hasBabyCarriageUnavailable"": false,
                        ""hasBabyCarriageAny"": false,
                        ""isOrangePartly"": false
                    },
                    {
                        ""journeyId"": 4,
                        ""code"": ""standard_billet_og_plads"",
                        ""ticketName"": ""Standard billet"",
                        ""ticketNameInDanish"": ""Standard billet"",
                        ""ticketClass"": 1,
                        ""sortIndex"": 24,
                        ""isAvailable"": false,
                        ""isMainProduct"": true,
                        ""isVisible"": false,
                        ""seatReservationPrice"": 0.0,
                        ""ticketPrice"": 0.0,
                        ""discountedPrice"": 0.0,
                        ""isSeatsSoldOut"": false,
                        ""isReservationRequired"": false,
                        ""hasTickets"": false,
                        ""hasSeatReservations"": false,
                        ""passengers"": [],
                        ""seatReservations"": 0,
                        ""seatingGroups"": [],
                        ""productNotifications"": [],
                        ""stretchInfo"": {
                            ""20"": 0
                        },
                        ""ticketDetail"": {
                            ""type"": 0,
                            ""immediatelyIsAvailable"": false
                        },
                        ""hasBabyCarriage"": false,
                        ""hasBabyCarriageUnavailable"": false,
                        ""hasBabyCarriageAny"": false,
                        ""isOrangePartly"": false
                    }
                ],
                ""himMessages"": [],
                ""id"": 2,
                ""searchId"": 1517898110,
                ""lowestPrice"": 0.0,
                ""numberShifts"": 0,
                ""arrivalDate"": ""2024-04-21T22:25:00+02:00"",
                ""arrivalZoneInfo"": {
                    ""zone"": 0,
                    ""regionCodeNumber"": 0,
                    ""regionPrefix"": ""unknown – no zone info"",
                    ""isZoneSpecified"": false,
                    ""displayInfo"": ""unknown – no zone info""
                },
                ""arrivalLocation"": ""Malmö C"",
                ""departureDate"": ""2024-04-21T21:44:00+02:00"",
                ""departureZoneInfo"": {
                    ""zone"": 1,
                    ""regionCodeNumber"": 1,
                    ""regionPrefix"": ""dot"",
                    ""isZoneSpecified"": true,
                    ""displayInfo"": ""dot 1""
                },
                ""departLocation"": ""København H"",
                ""status"": 0,
                ""journeyTime"": 41,
                ""ticketPrice"": 0.0,
                ""seatReservationPrice"": 0.0,
                ""isAvailable"": false,
                ""productNotifications"": [
                    {
                        ""heading"": ""Din valgte rejse kan ikke købes her"",
                        ""body"": ""<p><span>Da Sk&aring;netrafikken har overtaget str&aelig;kningen, kan billetten ikke k&oslash;bes i DSBs salgskanaler. For billetter fra Hovedstadsomr&aring;det til Sydsverige henviser vi til <a href=\""https://www.skanetrafiken.se/\"" target=\""_blank\"" rel=\""noopener\"">skanetrafiken.se</a></span></p>"",
                        ""location"": 2,
                        ""productCodes"": []
                    }
                ],
                ""hasAvailableOrangePartly"": false
            },
            {
                ""isLightningPlus"": false,
                ""numberOfSeats"": 0,
                ""isSeatsOnly"": false,
                ""isSeatReservationRequired"": false,
                ""stretches"": [
                    {
                        ""id"": 30,
                        ""lineName"": ""Regional-tog 1144"",
                        ""transportType"": {
                            ""id"": 23,
                            ""transportTypeFamily"": 1,
                            ""iconCssClass"": ""svg-train""
                        },
                        ""departLocation"": ""København H"",
                        ""departureDate"": ""2024-04-21T22:14:00+02:00"",
                        ""departureLocationIsInTS"": false,
                        ""departZoneInfo"": {
                            ""zone"": 1,
                            ""regionCodeNumber"": 1,
                            ""regionPrefix"": ""dot"",
                            ""isZoneSpecified"": true,
                            ""displayInfo"": ""dot 1""
                        },
                        ""arriveLocation"": ""Malmö C"",
                        ""arrivalDate"": ""2024-04-21T22:55:00+02:00"",
                        ""arrivalLocationIsInTS"": false,
                        ""arriveZoneInfo"": {
                            ""zone"": 0,
                            ""regionCodeNumber"": 0,
                            ""regionPrefix"": ""unknown – no zone info"",
                            ""isZoneSpecified"": false,
                            ""displayInfo"": ""unknown – no zone info""
                        },
                        ""isToFromBornholm"": false,
                        ""status"": 0
                    }
                ],
                ""ticketInstances"": [
                    {
                        ""journeyId"": 9,
                        ""code"": ""dsb_1_billet_og_plads"",
                        ""ticketName"": ""DSB 1' billet"",
                        ""ticketNameInDanish"": ""DSB 1' billet"",
                        ""ticketClass"": 2,
                        ""sortIndex"": 12,
                        ""isAvailable"": false,
                        ""isMainProduct"": true,
                        ""isVisible"": false,
                        ""seatReservationPrice"": 0.0,
                        ""ticketPrice"": 0.0,
                        ""discountedPrice"": 0.0,
                        ""isSeatsSoldOut"": false,
                        ""isReservationRequired"": false,
                        ""hasTickets"": false,
                        ""hasSeatReservations"": false,
                        ""passengers"": [],
                        ""seatReservations"": 0,
                        ""seatingGroups"": [],
                        ""productNotifications"": [],
                        ""stretchInfo"": {
                            ""30"": 0
                        },
                        ""ticketDetail"": {
                            ""type"": 0,
                            ""immediatelyIsAvailable"": false
                        },
                        ""hasBabyCarriage"": false,
                        ""hasBabyCarriageUnavailable"": false,
                        ""hasBabyCarriageAny"": false,
                        ""isOrangePartly"": false
                    },
                    {
                        ""journeyId"": 10,
                        ""code"": ""orange"",
                        ""ticketName"": ""DSB Orange billet"",
                        ""ticketNameInDanish"": ""DSB Orange billet"",
                        ""ticketClass"": 1,
                        ""sortIndex"": 22,
                        ""isAvailable"": false,
                        ""isMainProduct"": true,
                        ""isVisible"": false,
                        ""seatReservationPrice"": 0.0,
                        ""ticketPrice"": 0.0,
                        ""discountedPrice"": 0.0,
                        ""isSeatsSoldOut"": false,
                        ""isReservationRequired"": false,
                        ""hasTickets"": false,
                        ""hasSeatReservations"": false,
                        ""passengers"": [],
                        ""seatReservations"": 0,
                        ""seatingGroups"": [],
                        ""productNotifications"": [],
                        ""stretchInfo"": {
                            ""30"": 0
                        },
                        ""ticketDetail"": {
                            ""type"": 0,
                            ""immediatelyIsAvailable"": false
                        },
                        ""hasBabyCarriage"": false,
                        ""hasBabyCarriageUnavailable"": false,
                        ""hasBabyCarriageAny"": false,
                        ""isOrangePartly"": false
                    },
                    {
                        ""journeyId"": 11,
                        ""code"": ""orange_fri"",
                        ""ticketName"": ""DSB Orange Fri billet"",
                        ""ticketNameInDanish"": ""DSB Orange Fri billet"",
                        ""ticketClass"": 1,
                        ""sortIndex"": 23,
                        ""isAvailable"": false,
                        ""isMainProduct"": true,
                        ""isVisible"": false,
                        ""seatReservationPrice"": 0.0,
                        ""ticketPrice"": 0.0,
                        ""discountedPrice"": 0.0,
                        ""isSeatsSoldOut"": false,
                        ""isReservationRequired"": false,
                        ""hasTickets"": false,
                        ""hasSeatReservations"": false,
                        ""passengers"": [],
                        ""seatReservations"": 0,
                        ""seatingGroups"": [],
                        ""productNotifications"": [],
                        ""stretchInfo"": {
                            ""30"": 0
                        },
                        ""ticketDetail"": {
                            ""type"": 0,
                            ""immediatelyIsAvailable"": false
                        },
                        ""hasBabyCarriage"": false,
                        ""hasBabyCarriageUnavailable"": false,
                        ""hasBabyCarriageAny"": false,
                        ""isOrangePartly"": false
                    },
                    {
                        ""journeyId"": 8,
                        ""code"": ""standard_billet_og_plads"",
                        ""ticketName"": ""Standard billet"",
                        ""ticketNameInDanish"": ""Standard billet"",
                        ""ticketClass"": 1,
                        ""sortIndex"": 24,
                        ""isAvailable"": false,
                        ""isMainProduct"": true,
                        ""isVisible"": false,
                        ""seatReservationPrice"": 0.0,
                        ""ticketPrice"": 0.0,
                        ""discountedPrice"": 0.0,
                        ""isSeatsSoldOut"": false,
                        ""isReservationRequired"": false,
                        ""hasTickets"": false,
                        ""hasSeatReservations"": false,
                        ""passengers"": [],
                        ""seatReservations"": 0,
                        ""seatingGroups"": [],
                        ""productNotifications"": [],
                        ""stretchInfo"": {
                            ""30"": 0
                        },
                        ""ticketDetail"": {
                            ""type"": 0,
                            ""immediatelyIsAvailable"": false
                        },
                        ""hasBabyCarriage"": false,
                        ""hasBabyCarriageUnavailable"": false,
                        ""hasBabyCarriageAny"": false,
                        ""isOrangePartly"": false
                    }
                ],
                ""himMessages"": [],
                ""id"": 3,
                ""searchId"": 1517898110,
                ""lowestPrice"": 0.0,
                ""numberShifts"": 0,
                ""arrivalDate"": ""2024-04-21T22:55:00+02:00"",
                ""arrivalZoneInfo"": {
                    ""zone"": 0,
                    ""regionCodeNumber"": 0,
                    ""regionPrefix"": ""unknown – no zone info"",
                    ""isZoneSpecified"": false,
                    ""displayInfo"": ""unknown – no zone info""
                },
                ""arrivalLocation"": ""Malmö C"",
                ""departureDate"": ""2024-04-21T22:14:00+02:00"",
                ""departureZoneInfo"": {
                    ""zone"": 1,
                    ""regionCodeNumber"": 1,
                    ""regionPrefix"": ""dot"",
                    ""isZoneSpecified"": true,
                    ""displayInfo"": ""dot 1""
                },
                ""departLocation"": ""København H"",
                ""status"": 0,
                ""journeyTime"": 41,
                ""ticketPrice"": 0.0,
                ""seatReservationPrice"": 0.0,
                ""isAvailable"": false,
                ""productNotifications"": [
                    {
                        ""heading"": ""Din valgte rejse kan ikke købes her"",
                        ""body"": ""<p><span>Da Sk&aring;netrafikken har overtaget str&aelig;kningen, kan billetten ikke k&oslash;bes i DSBs salgskanaler. For billetter fra Hovedstadsomr&aring;det til Sydsverige henviser vi til <a href=\""https://www.skanetrafiken.se/\"" target=\""_blank\"" rel=\""noopener\"">skanetrafiken.se</a></span></p>"",
                        ""location"": 2,
                        ""productCodes"": []
                    }
                ],
                ""hasAvailableOrangePartly"": false
            }
        ],
        ""canSearchEarlier"": true,
        ""canSearchLater"": true,
        ""searchType"": 1
    }
]";

        }
    }

}
public class Criteria
{
    public int Direction { get; set; }
    public Location DepartLocation { get; set; }
    public Location ArriveLocation { get; set; }
    public string SearchDate { get; set; }
    public string SearchTime { get; set; }
    public int Type { get; set; }
    public int SearchType { get; set; }
    public int SeatReservations { get; set; }
    public int PassengersAdults { get; set; }
    public int PassengersAdultsDiscount { get; set; }
    public int PassengersChildren { get; set; }
    public int PassengersChildrenDiscount { get; set; }
    public int PassengersYoungsters { get; set; }
    public int PassengersYoungstersDiscount { get; set; }
    public int PassengersSeniors { get; set; }
    public int PassengersSeniorsDiscount { get; set; }
    public int PassengersWildcard { get; set; }
    public int PassengersWildcardDiscount { get; set; }
    public bool LimitTrainOnly { get; set; }
    public bool RequestBabyCarriage { get; set; }
    public int JourneyId { get; set; }
}

public class Location
{
    public string Name { get; set; }
    public int UicNumber { get; set; }
    public string TransportAuthority { get; set; }
}

public class NetbutikSearch
{
    public List<Criteria> Criteria { get; set; }
}