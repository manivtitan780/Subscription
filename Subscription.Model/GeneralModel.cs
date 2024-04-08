using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace Subscription.Model
{
    internal static class GeneralModel
    {
        static GeneralModel()
        {
            lock (Lock)
            {
                IConfigurationBuilder _builder =
                    new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true);

                IConfigurationRoot _configuration = _builder.Build();

                APIHost = _configuration.GetSection("APIHost").Value;
            }
        }

        /// <summary>
        ///     The object used for locking to ensure thread safety when initializing the General class.
        /// </summary>
        private static readonly object Lock = new();

        /// <summary>
        ///     Gets the API host value from the configuration.
        /// </summary>
        /// <value>
        ///     The API host as a string.
        /// </value>
        /// <remarks>
        ///     This property is used to configure the base address for REST client instances in various parts of the application.
        /// </remarks>
        public static string APIHost
        {
            get;
        }

    }
}
