#region Header

// /*****************************************
// Copyright:           Titan-Techs.
// Location:            Newtown, PA, USA
// Solution:            Subscription
// Project:             Subscription.Server
// File Name:           ZipCodes.cs
// Created By:          Narendra Kumaran Kadhirvelu, Jolly Joseph Paily, DonBosco Paily, Mariappan Raja, Gowtham Selvaraj, Pankaj Sahu, Brijesh Dubey
// Created On:          04-16-2025 15:04
// Last Updated On:     04-16-2025 16:04
// *****************************************/

#endregion

namespace Subscription.Server.Code;

public class ZipCodeService(IMemoryCache cache, RedisService redisService)
{
    public Task<List<KeyValues>> GetZipCodes() => cache.GetOrCreateAsync("ZipCodes", async entry =>
                                                                                     {
                                                                                         entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(60);

                                                                                         RedisValue _value = await redisService.GetAsync(nameof(CacheObjects.Zips));

                                                                                         if (_value.IsNullOrEmpty || _value == "[]")
                                                                                         {
                                                                                             return [];
                                                                                         }

                                                                                         List<Zip> _zips = General.DeserializeObject<List<Zip>>(_value.ToString());

                                                                                         return _zips.Select(zip => new KeyValues
                                                                                                                    {
                                                                                                                        KeyValue = zip.ZipCode, Text = zip.ZipCode
                                                                                                                    }).ToList();
                                                                                     });
}