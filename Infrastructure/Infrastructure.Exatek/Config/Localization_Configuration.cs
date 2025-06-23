using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Models.Entities.Localization;

namespace Infrastructure.Data.Config
{
    public class Localization_Configuration
    {
        public Localization_Configuration( ModelBuilder modelBuilder) {
            // Configure localization relationships
            modelBuilder.Entity<DictionaryLocalization>()
                .HasKey(dl => new { dl.ID, dl.LanguageID });

            modelBuilder.Entity<DictionaryLocalization>()
                .HasOne(dl => dl.Dictionary)
                .WithMany(d => d.DictionaryLocalization)
                .HasForeignKey(dl => dl.ID);

            SeedLanguages(modelBuilder);
            SeedDictionaries(modelBuilder);.
            SeedDictionaryLocalizations(modelBuilder);
        }
        private void SeedLanguages(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Languages>().HasData(
                new Languages { ID = 1, LanguageName = "en", Description = "English", Direction = "LTR", Culture = "en-US" },
                new Languages { ID = 2, LanguageName = "ar", Description = "Arabic", Direction = "RTL", Culture = "ar-SA" }
            );
        }


        private void SeedDictionaries(ModelBuilder modelBuilder)
        {
            var dictionaries = Enumerable.Range(0, 1000).Select(i => new Dictionary<string, string>()).ToArray();
            modelBuilder.Entity<Dictionary>().HasData(dictionaries);
        }

        private void SeedDictionaryLocalizations(ModelBuilder modelBuilder)
        {
            var localizations = new List<DictionaryLocalization>();

            // English Localizations
            var englishLocalizations = new Dictionary<int, string>
            {
                // Common
                { 1, "Welcome" }, { 2, "Continue" }, { 3, "Next" }, { 4, "Back" },
                { 5, "Submit" }, { 6, "Verify" }, { 7, "Resend" }, { 8, "Complete" },

                // Registration Steps
                { 20, "Registration Steps" }, { 21, "Personal Information" }, { 22, "Mobile Verification" },
                { 23, "Email Verification" }, { 24, "Policy Approval" }, { 25, "PIN Setup" },
                { 26, "Biometric Setup" }, { 27, "Completed" },

                // Step Descriptions
                { 30, "Enter your personal details" },
                { 31, "Verify your mobile number" },
                { 32, "Verify your email address" },
                { 33, "Accept terms and conditions" },
                { 34, "Create your security PIN" },
                { 35, "Set up biometric authentication" },
                { 36, "Registration completed successfully" },

                // Form Fields
                { 50, "IC Number" }, { 51, "Full Name" }, { 52, "Email Address" },
                { 53, "Phone Number" }, { 54, "Language" },

                // OTP & Verification
                { 70, "Enter OTP" }, { 71, "OTP sent to your mobile" }, { 72, "OTP expired" },
                { 73, "Invalid OTP" }, { 74, "OTP verified successfully" }, { 75, "Resend OTP" },

                // Policy & Terms
                { 80, "Terms and Conditions" }, { 81, "Privacy Policy" }, { 82, "I agree to the terms and conditions" },
                { 83, "Policy accepted" },

                // PIN Setup
                { 90, "Create PIN" }, { 91, "Confirm PIN" }, { 92, "PIN numbers do not match" },
                { 93, "PIN created successfully" }, { 94, "Enter 6-digit PIN" },

                // Biometric
                { 100, "Enable Biometric Authentication" }, { 101, "Biometric Setup" }, { 102, "Face ID" },
                { 103, "Touch ID" }, { 104, "Biometric authentication enabled" }, { 105, "Skip Biometric" },

                // Messages
                { 120, "Personal information saved" }, { 121, "Mobile number verified" }, { 122, "Email verified" },
                { 123, "Registration completed successfully" }, { 124, "Validation error" }, { 125, "This field is required" },
                { 126, "Invalid IC Number" }, { 127, "IC Number already exists" },

                // Placeholders
                { 140, "Enter your IC Number" }, { 141, "Enter your full name" }, { 142, "Enter your email" },
                { 143, "Enter your phone number" }, { 144, "Select your language" },

                 { 145, "This field is required and Length is between {0} and {1}" }, { 146, "Please enter a valid e-mail address" }
            };

            // Arabic Localizations
            var arabicLocalizations = new Dictionary<int, string>
            {
                // Common
                { 1, "مرحباً" }, { 2, "متابعة" }, { 3, "التالي" }, { 4, "رجوع" },
                { 5, "إرسال" }, { 6, "تحقق" }, { 7, "إعادة إرسال" }, { 8, "إكمال" },

                // Registration Steps
                { 20, "خطوات التسجيل" }, { 21, "المعلومات الشخصية" }, { 22, "التحقق من الجوال" },
                { 23, "التحقق من البريد الإلكتروني" }, { 24, "الموافقة على السياسة" }, { 25, "إعداد الرقم السري" },
                { 26, "إعداد البيانات الحيوية" }, { 27, "مكتمل" },

                // Step Descriptions
                { 30, "أدخل بياناتك الشخصية" },
                { 31, "تحقق من رقم جوالك" },
                { 32, "تحقق من عنوان بريدك الإلكتروني" },
                { 33, "اقبل الشروط والأحكام" },
                { 34, "أنشئ رقمك السري للحماية" },
                { 35, "قم بإعداد المصادقة البيومترية" },
                { 36, "تم إكمال التسجيل بنجاح" },

                // Form Fields
                { 50, "رقم الهوية" }, { 51, "الاسم الكامل" }, { 52, "البريد الإلكتروني" },
                { 53, "رقم الهاتف" }, { 54, "اللغة" },

                // OTP & Verification
                { 70, "أدخل رمز التحقق" }, { 71, "تم إرسال رمز التحقق إلى جوالك" }, { 72, "انتهت صلاحية رمز التحقق" },
                { 73, "رمز تحقق غير صحيح" }, { 74, "تم التحقق من الرمز بنجاح" }, { 75, "إعادة إرسال الرمز" },

                // Policy & Terms
                { 80, "الشروط والأحكام" }, { 81, "سياسة الخصوصية" }, { 82, "أوافق على الشروط والأحكام" },
                { 83, "تم قبول السياسة" },

                // PIN Setup
                { 90, "إنشاء الرقم السري" }, { 91, "تأكيد الرقم السري" }, { 92, "الأرقام السرية غير متطابقة" },
                { 93, "تم إنشاء الرقم السري بنجاح" }, { 94, "أدخل الرقم السري المكون من 6 أرقام" },

                // Biometric
                { 100, "تفعيل المصادقة البيومترية" }, { 101, "إعداد البيانات الحيوية" }, { 102, "التعرف على الوجه" },
                { 103, "بصمة اللمس" }, { 104, "تم تفعيل المصادقة البيومترية" }, { 105, "تخطي البيانات الحيوية" },

                // Messages
                { 120, "تم حفظ المعلومات الشخصية" }, { 121, "تم التحقق من رقم الجوال" }, { 122, "تم التحقق من البريد الإلكتروني" },
                { 123, "تم إكمال التسجيل بنجاح" }, { 124, "خطأ في التحقق" }, { 125, "هذا الحقل مطلوب" },
                { 126, "رقم هوية غير صحيح" }, { 127, "رقم الهوية موجود مسبقاً" },

                // Placeholders
                { 140, "أدخل رقم الهوية" }, { 141, "أدخل اسمك الكامل" }, { 142, "أدخل بريدك الإلكتروني" },
                { 143, "أدخل رقم هاتفك" }, { 144, "اختر لغتك" },

                { 145, "هذا الحقل مطلوب ويجب ان يكون عدد الخانات بين {0} و {1}" }, { 146, "الرجاء ادخال بريد الكتروني صالح" }

            };

            // Add English localizations
            foreach (var item in englishLocalizations)
            {
                localizations.Add(new DictionaryLocalization
                {
                    ID = item.Key,
                    LanguageID = 1,
                    Description = item.Value
                });
            }

            // Add Arabic localizations
            foreach (var item in arabicLocalizations)
            {
                localizations.Add(new DictionaryLocalization
                {
                    ID = item.Key,
                    LanguageID = 2,
                    Description = item.Value
                });
            }

            modelBuilder.Entity<DictionaryLocalization>().HasData(localizations);
        }
    }
}
