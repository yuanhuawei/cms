﻿using MaiDar.Core;
using MaiDarServer.CMS.Core;
using MaiDarServer.CMS.Model;
using MaiDarServer.CMS.Model.Enumerations;
using System;

namespace MaiDarServer
{
    public class ExecutionUtils
    {
        public static void LogError(TaskInfo taskInfo, Exception ex)
        {
            if (taskInfo != null && ex != null)
            {
                var logInfo = new TaskLogInfo(0, taskInfo.TaskID, false, ex.Message, DateTime.Now);
                DataProvider.TaskLogDao.Insert(logInfo);
            }
        }

        public static bool IsNeedExecute(TaskInfo taskInfo)
        {
            if (taskInfo != null && taskInfo.IsEnabled)
            {
                var now = DateTime.Now;
                var dateDiff = now - taskInfo.LastExecuteDate;

                if (taskInfo.FrequencyType == EFrequencyType.Month)
                {
                    if (dateDiff.TotalDays >= 28 && now.Day == taskInfo.StartDay && now.Hour == taskInfo.StartHour)
                    {
                        return true;
                    }
                }
                else if (taskInfo.FrequencyType == EFrequencyType.Week)
                {
                    if (dateDiff.TotalDays >= 7 && DateUtils.GetDayOfWeek(now) == taskInfo.StartWeekday && now.Hour == taskInfo.StartHour)
                    {
                        return true;
                    }
                }
                else if (taskInfo.FrequencyType == EFrequencyType.Day)
                {
                    if (dateDiff.TotalDays >= 1 && now.Hour == taskInfo.StartHour)
                    {
                        return true;
                    }
                }
                else if (taskInfo.FrequencyType == EFrequencyType.Hour)
                {
                    if (dateDiff.TotalHours >= 1)
                    {
                        return true;
                    }
                }
                else if (taskInfo.FrequencyType == EFrequencyType.Period)
                {
                    if (dateDiff.TotalMinutes >= taskInfo.PeriodIntervalMinute)
                    {
                        return true;
                    }
                }
                else if (taskInfo.FrequencyType == EFrequencyType.OnlyOnce)
                {
                    if (taskInfo.ServiceType == EServiceType.Create && taskInfo.OnlyOnceDate <= DateTime.Now)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
