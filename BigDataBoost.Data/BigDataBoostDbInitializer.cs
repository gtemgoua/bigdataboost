using BigDataBoost.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigDataBoost.Data
{
    public class BigDataBoostDbInitializer
    {
        private static BigDataBoostContext context;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            context = (BigDataBoostContext)serviceProvider.GetService(typeof(BigDataBoostContext));

            InitializeTaxonomy();
 
        }

        private static void InitializeTaxonomy()
        {
            if (!context.Tags.Any())
            {
                TagDef pt_01 = new TagDef { Source = "eDNA", Name = "PRISMLAKE_01", Description = "Prism Data Lake Point 01", ExtendedDescription = "Prism Data Lake Point 01 Extended Description" };
                TagDef pt_02 = new TagDef { Source = "pi", Name = "PRISMLAKE_02", Description = "Prism Data Lake Point 02", ExtendedDescription = "Prism Data Lake Point 02 Extended Description" };
                TagDef pt_03 = new TagDef { Source = "ODBC", Name = "PRISMLAKE_03", Description = "Prism Data Lake Point 03", ExtendedDescription = "Prism Data Lake Point 03 Extended Description" };
                TagDef pt_04 = new TagDef { Source = "eDNA", Name = "PRISMLAKE_04", Description = "Prism Data Lake Point 04", ExtendedDescription = "Prism Data Lake Point 04 Extended Description" };
                TagDef pt_05 = new TagDef { Source = "pi", Name = "PRISMLAKE_05", Description = "Prism Data Lake Point 05", ExtendedDescription = "Prism Data Lake Point 05 Extended Description" };
                TagDef pt_06 = new TagDef { Source = "eDNA", Name = "PRISMLAKE_06", Description = "Prism Data Lake Point 06", ExtendedDescription = "Prism Data Lake Point 06 Extended Description" };

                context.Tags.Add(pt_01);
                context.Tags.Add(pt_02);
                context.Tags.Add(pt_03);
                context.Tags.Add(pt_04);
                context.Tags.Add(pt_05);
                context.Tags.Add(pt_06);

                context.SaveChanges();
            }

            if (!context.Historian.Any())
            {
                double startAngle = 0.0;
                List<DateTime> times = new List<DateTime>();
                DateTime start = DateTime.UtcNow;

                for (int i = 0; i < 10; i++)
                    times.Add(start.AddHours(-1 * i));

                for (int pointIndex = 1; pointIndex < 7; pointIndex++)
                {
                    startAngle = 0.0;
                    for (int recIndex = 0; recIndex < 12; recIndex++)
                    {
                        TagHist hist_x = new TagHist
                        {
                            TagDefId = pointIndex,
                            TagName = context.Tags.FirstOrDefault(t => t.Id == pointIndex).Name,
                            TimeStamp = times[recIndex],
                            Value = GenerateValue(pointIndex, startAngle)
                        };
                        startAngle += 30;
                        context.Historian.Add(hist_x);
                    }
                }
            }

            context.SaveChanges();
        }

        private static string[] functions = { "sine", "cosine" };
        private static double cursorAngle = 0.0;
        private static double GenerateValue(int tagIndex, double angle)
        {
            double result = 0.0;
            int funcIndex = tagIndex % 2;
            double radAngle = Math.PI * angle / 180.0;

            switch (functions[funcIndex])
            {
                case "sine":
                    result = Math.Sin(radAngle);
                    break;
                case "cosine":
                    result = Math.Cos(radAngle);
                    break;
            }
            return result;
        }

        public static void GenerateRunTimeData(TagStatus pstatus)
        {
            // Update all tags with current values
            var tags = context.Tags;
            DateTime ts = DateTime.UtcNow;
            // remove milliseconds
            ts = ts.AddTicks(-(ts.Ticks % TimeSpan.TicksPerSecond));

            foreach (var item in tags)
            {
                int tagIndex = tags.ToList().IndexOf(item);

                item.Value = GenerateValue(tagIndex,cursorAngle);
                item.TimeStamp = ts;
                item.Status = pstatus;

                TagHist histRecord = new TagHist
                {
                    TagDefId = item.Id,
                    TagName = item.Name,
                    Value = item.Value,
                    Status = pstatus,
                    TimeStamp = ts
                };
                context.Historian.Add(histRecord);
            }
            context.SaveChanges();
            cursorAngle = (cursorAngle + 5) % 360;
        }
    }
}
