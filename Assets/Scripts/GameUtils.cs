using System;
using System.Collections.Generic;
using System.Globalization;
using UniState;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyGameNamespace
{
    public class GameUtils
    {
        public class DefaultResolver : ITypeResolver
        {
            public object Resolve(Type type)
            {
                // Nếu có constructor rỗng thì tạo bằng Activator
                if (type.GetConstructor(Type.EmptyTypes) != null)
                    return Activator.CreateInstance(type);

                // Nếu không biết cách tạo thì trả null
                return null;
            }
        }
        //format kiểu tycoon K/M/B/T với tối đa 2 số lẻ, và giữ hiển thị gọn cho số nhỏ. 
        // Ví dụ: 999 -> "999", 1500 -> "1.5K", 2000000 -> "2M", 3500000000 -> "3.5B"
        public static string FormatNumber(double value)
        {
            string[] suffixes = { "", "K", "M", "B", "T", "aa", "ab", "ac" };
            double absValue = Math.Abs(value);
            int suffixIndex = 0;

            while (absValue >= 1000d && suffixIndex < suffixes.Length - 1)
            {
                absValue /= 1000d;
                suffixIndex++;
            }

            double scaledValue = value / Math.Pow(1000d, suffixIndex);
            double roundedValue = Math.Round(scaledValue, 2, MidpointRounding.AwayFromZero);

            if (Math.Abs(roundedValue) >= 1000d && suffixIndex < suffixes.Length - 1)
            {
                roundedValue /= 1000d;
                suffixIndex++;
            }

            return roundedValue.ToString("0.##", CultureInfo.InvariantCulture) + suffixes[suffixIndex];
        }

        // Format kiểu đầy đủ, chỉ thêm dấu "." để phân tách hàng nghìn.
        // Ví dụ: 999 -> "999", 1500 -> "1.500", 2000000 -> "2.000.000"
        public static string FormatNumberWithDots(double value)
        {
            long roundedValue = (long)Math.Round(value, MidpointRounding.AwayFromZero);
            string formatted = Math.Abs(roundedValue).ToString("#,0", CultureInfo.InvariantCulture).Replace(',', '.');
            return roundedValue < 0 ? "-" + formatted : formatted;
        }

        public static bool IsPointerOverUIObject()
        {

            // return EventSystem.current.IsPointerOverGameObject();

            // Tạo PointerEventData dựa trên vị trí chuột
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = Input.mousePosition;

            // Danh sách kết quả raycast
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }


        /// <summary>
        /// Format the time span to a string in the format "HH:MM:SS".
        /// </summary>
        /// <param name="timeSpan">The time span to format.</param>
        /// <returns>A string representing the time span.</returns>
        public static string FormatLongTimeSpan(TimeSpan timeSpan)
        {
            // Tính tổng số giờ (bao gồm cả số ngày chuyển đổi thành giờ)
            int totalHours = (int)timeSpan.TotalHours;

            return string.Format("{0:D2}:{1:D2}:{2:D2}",
                totalHours,
                timeSpan.Minutes,
                timeSpan.Seconds);
        }

        public static string FormatCountdownTime(long totalSeconds, bool showHours = true)
        {
            TimeSpan time = TimeSpan.FromSeconds(Math.Max(0, totalSeconds));
            if (showHours)
                return $"{(long)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}";

            return $"{(long)time.TotalMinutes:00}:{time.Seconds:00}";
        }

        public static string FormatHourMinuteTime(long totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(Math.Max(0, totalSeconds));
            long totalHours = (long)time.TotalHours;

            if (totalHours > 0)
                return $"{totalHours}h {time.Minutes}m";

            return $"{(long)time.TotalMinutes}m";
        }

        public static string FormatPercent(float value)
        {
            // "0.##" -> Nếu là 10.512 -> "10.51", nếu là 10.0 -> "10"
            return value.ToString("0.##") + "%";
        }

        static string starIcon = "icon_star";
        static string coinIcon = "icon_coin";
        public static string GetStarIconSpriteString() => GetIconSpriteString(starIcon);
        public static string GetStarIconSpriteString(float size, float voffset) => GetIconSpriteString(starIcon, size, voffset);
        public static string GetCoinIconSpriteString() => GetIconSpriteString(coinIcon);
        public static string GetCoinIconSpriteString(float size, float voffset) => $"<size={size}><voffset={voffset}><sprite name=\"{coinIcon}\"></voffset></size>";

        public static string GetIconSpriteString(string iconName, float size, float voffset) => $"<size={size}><voffset={voffset}><sprite name=\"{iconName}\"></voffset></size>";
        public static string GetIconSpriteString(string iconName) => $"<sprite name=\"{iconName}\">";
    }
}
