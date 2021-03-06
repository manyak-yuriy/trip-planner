﻿using System;
using System.Collections.Generic;
using Booking.Implementation.Enums;
using Booking.Implementation.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace Booking.Implementation
{
    public class BookingAccessor : IDisposable
    {
        private readonly IWebDriver _driver;

        public BookingAccessor(IWebDriver webDriver)
        {
            _driver = webDriver;
            _driver.Url = "https://booking.com";
        }

        #region Methods

        public void SetDirection(string direction)
        {
            try
            {
                var webElement = _driver.FindElement(By.Name("ss"));
                webElement.Clear();
                webElement.SendKeys(direction);
            }
            catch (Exception e)
            {
                //loger.Log
            }
        }

        public void SetTravelingForWork(bool forWork)
        {
            try
            {
                var container = _driver.FindElement(By.ClassName("b-form-group-content__container"));
                var radiobuttons = container.FindElements(By.ClassName("b-booker-type__input"));
                var executableRadioBtn = forWork ? radiobuttons[0] : radiobuttons[1];
                ((IJavaScriptExecutor) _driver).ExecuteScript("arguments[0].checked = true;", executableRadioBtn);
            }
            catch (Exception e)
            {
                //loger.Log
            }
        }

        public void SetVisitors(int adults, int children)
        {
            if (adults <= 0 || children < 0)
            {
                return;
            }
            try
            {
                _driver.FindElement(By.Id("group_adults"))
                    .FindElement(By.CssSelector($"option[value='{adults}']"))
                    .Click();
                _driver.FindElement(By.Id("group_children"))
                    .FindElement(By.CssSelector($"option[value='{children}']"))
                    .Click();
            }
            catch (Exception e)
            {
                //loger.Log
            }
        }

        public void SetCheckInDate(DateTime date)
        {
            try
            {
                var control = _driver.FindElement(By.ClassName("sb-dates__grid"));
                var dateControl = control.FindElements(By.ClassName("sb-date-field__controls"));
                SetDate(dateControl[0], date, "in");
            }
            catch (Exception e)
            {
                //loger.Log
            }
        }

        public void SetCheckOutDate(DateTime date)
        {
            try
            {
                var control = _driver.FindElement(By.ClassName("sb-dates__grid"));
                var dateControl = control.FindElements(By.ClassName("sb-date-field__controls"));
                SetDate(dateControl[1],date, "out");
            }
            catch (Exception e)
            {
                //loger.Log
            }
        }

        private void SetDate(IWebElement dateControl, DateTime date, string par)
        {
            var month = dateControl.FindElement(By.Name($"check{par}_month"));
            month.Clear();
            month.SendKeys(date.Month.ToString());

            var day = dateControl.FindElement(By.Name($"check{par}_monthday"));
            day.Clear();
            day.SendKeys(date.Day.ToString());

            var year = dateControl.FindElement(By.Name($"check{par}_year"));
            year.Clear();
            year.SendKeys(date.Year.ToString());

        }

        public void SetArrivingMethod(ArrivingMethod arriving)
        {
            try
            {
                var container = _driver.FindElement(By.ClassName("b-sb-travelling-types__options"));
                var radiobuttons = container.FindElements(By.ClassName("b-sb-travelling-types__input"));

                var executableRadioBtn = radiobuttons[0];
                switch (arriving)
                {
                    case ArrivingMethod.Aeroplane:
                        executableRadioBtn = radiobuttons[0];
                        break;
                    case ArrivingMethod.Train:
                        executableRadioBtn = radiobuttons[1];
                        break;
                    case ArrivingMethod.Car:
                        executableRadioBtn = radiobuttons[2];
                        break;
                }
                ((IJavaScriptExecutor) _driver).ExecuteScript("arguments[0].checked = true;", executableRadioBtn);
            }
            catch (Exception e)
            {
                //loger.Log
            }
        }


        private void Search()
        {
            try
            {
                _driver.FindElement(By.ClassName("sb-searchbox__button")).Submit();
            }
            catch (Exception e)
            {
                //loger.Log
            }
        }

        public int GetTripCount()
        {
            var tripCount = 0;
            try
            {
                Search();
                tripCount = _driver.FindElements(By.ClassName("sr-hotel__name")).Count;
            }
            catch (Exception e)
            {
                //loger.Log
            }
            return tripCount;
        }

        public HotelsWrapper GetHotels()
        {
            var hotelsWrapper = new HotelsWrapper();
            var hotels = new List<HotelModel>(16);
            try
            {
                Search();
                foreach (var hotel in _driver.FindElements(By.ClassName("sr_item")))
                {
                    hotels.Add(GetHotelModel(hotel));
                }
                hotelsWrapper.Hotels = hotels;
            }
            catch (Exception e)
            {
                //loger.Log
            }
            return hotelsWrapper;
        }

        private HotelModel GetHotelModel(IWebElement hotelControl)
        {
            var hotel = new HotelModel();
            hotel.Name = hotelControl.TryFindElement(By.ClassName("sr-hotel__name"))?.Text;
            hotel.Description = hotelControl.TryFindElement(By.ClassName("hotel_desc"))?.Text;

            var imageContol = hotelControl.TryFindElement(By.ClassName("hotel_image"));
            var imageLink = imageContol?.GetAttribute("src");
            hotel.ImageLink = CreateUri(imageLink);

            var bookingHotelRawLink = hotelControl.TryFindElement(By.ClassName("hotel_name_link"))?.GetAttribute("href");
            hotel.HotelBookingUri = CreateUri(bookingHotelRawLink);

            hotel.RawPrice = hotelControl.TryFindElement(By.ClassName("site_price"))?.Text;

            // temporary

            hotel.Price = new Random().Next(300, 600);
            hotel.RawPrice = hotel.Price.ToString();

            hotel.Score = GetNumericData(hotelControl, "average");
            //hotel.MaxScore = GetNumericData(hotelControl, "bestRating", "content");
            return hotel;
        }

        private Uri CreateUri(string rawLink)
        {

            Uri uri = null;
            if (!string.IsNullOrEmpty(rawLink))
                Uri.TryCreate(rawLink, UriKind.RelativeOrAbsolute, out uri);
            return uri;
        }

        private double GetNumericData(IWebElement hotelControl, string className, string attribute = "")
        {
            double data = 0;
            var element = hotelControl.TryFindElement(By.ClassName(className));
            string strData = string.IsNullOrEmpty(attribute) ? element?.Text : element?.GetAttribute(attribute);
            double.TryParse(strData, out data);
            return data;
        }

        #endregion

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}