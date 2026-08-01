// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Arabic (`ar`).
class AppLocalizationsAr extends AppLocalizations {
  AppLocalizationsAr([String locale = 'ar']) : super(locale);

  @override
  String get appName => 'عُهدة';

  @override
  String get arabicAppName => 'عُهدة';

  @override
  String get tagline => 'مساءلة واضحة لعمليات الإنشاء';

  @override
  String get welcomeTitle => 'بداية واضحة لشركتك';

  @override
  String get welcomeBody => 'سجّل الدخول أو أنشئ شركة أو انضم إلى فريقك بأمان.';

  @override
  String get login => 'تسجيل الدخول';

  @override
  String get createCompany => 'إنشاء شركة';

  @override
  String get submitJoinRequest => 'طلب الانضمام';

  @override
  String get acceptInvitation => 'قبول دعوة';

  @override
  String get phoneNumber => 'رقم الهاتف';

  @override
  String get phoneHint => 'استخدم الصيغة الدولية E.164، مثال: +218912345678';

  @override
  String get password => 'كلمة المرور';

  @override
  String get showPassword => 'إظهار كلمة المرور';

  @override
  String get hidePassword => 'إخفاء كلمة المرور';

  @override
  String get companyName => 'اسم الشركة';

  @override
  String get companyCode => 'رمز الشركة';

  @override
  String get companyCodeHint => 'من 6 إلى 20 حرفًا إنجليزيًا كبيرًا أو رقمًا';

  @override
  String get fullName => 'الاسم الكامل';

  @override
  String get managerFullName => 'الاسم الكامل للمدير';

  @override
  String get managerPhone => 'رقم هاتف المدير';

  @override
  String get emailOptional => 'البريد الإلكتروني (اختياري)';

  @override
  String get requestedRole => 'الدور المطلوب';

  @override
  String get requestMessageOptional => 'رسالة إلى المدير (اختيارية)';

  @override
  String get requestRoleNotice =>
      'الدور المطلوب مبدئي ويجب أن يوافق عليه مدير الشركة.';

  @override
  String get identityRoleNotice =>
      'قد يتطلب دور النائب أو المحاسب التحقق من الهوية.';

  @override
  String get invitationToken => 'رمز الدعوة';

  @override
  String get invitationTokenHint => 'أدخل الرمز الذي تمت مشاركته معك';

  @override
  String get requiredField => 'هذا الحقل مطلوب.';

  @override
  String get invalidPhone => 'أدخل رقم هاتف صحيحًا بصيغة E.164.';

  @override
  String get invalidEmail => 'أدخل بريدًا إلكترونيًا صحيحًا.';

  @override
  String get invalidCompanyCode =>
      'استخدم من 6 إلى 20 حرفًا إنجليزيًا أو رقمًا.';

  @override
  String get passwordMinLogin => 'أدخل كلمة المرور.';

  @override
  String get passwordMinOnboarding => 'استخدم من 12 إلى 128 حرفًا.';

  @override
  String get invalidFullName => 'استخدم من 1 إلى 200 حرف من دون أسطر جديدة.';

  @override
  String get invalidCompanyName => 'استخدم من 1 إلى 200 حرف من دون أسطر جديدة.';

  @override
  String get invalidMessage => 'استخدم 1000 حرف كحد أقصى من دون أسطر جديدة.';

  @override
  String get invalidInvitationToken => 'أدخل رمز دعوة صحيحًا.';

  @override
  String get invalidRole => 'اختر دورًا متاحًا.';

  @override
  String get manager => 'مدير';

  @override
  String get deputy => 'نائب';

  @override
  String get accountant => 'محاسب';

  @override
  String get supervisor => 'مشرف';

  @override
  String get worker => 'عامل';

  @override
  String get unknownValue => 'غير معروف';

  @override
  String get statusActive => 'نشط';

  @override
  String get statusPendingApproval => 'في انتظار الموافقة';

  @override
  String get statusSuspended => 'موقوف';

  @override
  String get statusInactive => 'غير نشط';

  @override
  String get statusRejected => 'مرفوض';

  @override
  String get identityNotRequired => 'التحقق من الهوية غير مطلوب';

  @override
  String get identityPending => 'التحقق من الهوية قيد الانتظار';

  @override
  String get identityVerified => 'تم التحقق من الهوية';

  @override
  String get identityRejected => 'تم رفض التحقق من الهوية';

  @override
  String get pendingTitle => 'الخطوة التالية قيد الانتظار';

  @override
  String get joinPendingTitle => 'تم إرسال الطلب';

  @override
  String get joinPendingBody => 'طلبك في انتظار مراجعة مدير الشركة.';

  @override
  String get identityPendingTitle => 'التحقق من الهوية مطلوب';

  @override
  String get identityPendingBody =>
      'تم قبول دعوتك. يلزم التحقق من الهوية قبل تفعيل هذا الحساب.';

  @override
  String get approvalPendingTitle => 'موافقة المدير مطلوبة';

  @override
  String get approvalPendingBody => 'حسابك في انتظار موافقة الشركة.';

  @override
  String get genericPendingBody =>
      'تم استلام خطوة الانضمام وهي بانتظار الاكتمال.';

  @override
  String get backToLogin => 'العودة إلى تسجيل الدخول';

  @override
  String get homeFoundationTitle => 'الأساس الآمن للمصادقة';

  @override
  String get homeFoundationBody =>
      'جلسة عُهدة الآمنة جاهزة. وحدات الأعمال والمال ليست ضمن هذه المرحلة.';

  @override
  String get company => 'الشركة';

  @override
  String get role => 'الدور';

  @override
  String get accountStatus => 'حالة الحساب';

  @override
  String get companyStatus => 'حالة الشركة';

  @override
  String get logout => 'تسجيل الخروج';

  @override
  String get logoutExplanation =>
      'يسحب تسجيل الخروج رمز الوصول من هذا الجهاز ولا يلغيه على الخادم.';

  @override
  String get sessionExpired => 'انتهت جلستك. سجّل الدخول من جديد.';

  @override
  String get connectionUnavailableTitle => 'الاتصال غير متاح';

  @override
  String get connectionUnavailableBody =>
      'تعذر التحقق من جلستك. لم نحذف الجلسة المحفوظة على الهاتف.';

  @override
  String get retry => 'إعادة المحاولة';

  @override
  String get genericError => 'حدث خطأ. حاول مرة أخرى.';

  @override
  String get networkError => 'تحقق من اتصالك وحاول مرة أخرى.';

  @override
  String get timeoutError => 'استغرق الطلب وقتًا طويلًا. حاول مرة أخرى.';

  @override
  String get cancelledError => 'تم إلغاء الطلب.';

  @override
  String get invalidCredentials => 'رقم الهاتف أو كلمة المرور غير صحيحة.';

  @override
  String get validationError => 'راجع الحقول المحددة وحاول مرة أخرى.';

  @override
  String get conflictError =>
      'لا يمكن استخدام هذه البيانات. راجعها أو تواصل مع شركتك.';

  @override
  String get unauthorizedError => 'لم تعد جلستك صالحة.';

  @override
  String get serverError => 'الخدمة غير متاحة مؤقتًا. حاول لاحقًا.';

  @override
  String get invitationInvalid => 'رمز الدعوة غير صحيح أو لا يمكن قبوله.';

  @override
  String get joinUnavailable => 'لا يمكن إرسال طلب الانضمام بهذه البيانات.';

  @override
  String get registrationConflict =>
      'لا يمكن تسجيل الشركة أو المدير بهذه البيانات.';

  @override
  String get changeLanguage => 'تغيير اللغة';

  @override
  String get english => 'English';

  @override
  String get arabic => 'العربية';

  @override
  String get continueAction => 'متابعة';

  @override
  String get configurationTitle => 'إعداد التطوير مطلوب';

  @override
  String get configurationBody =>
      'القيمة API_BASE_URL مفقودة أو غير صحيحة. شغّل Flutter باستخدام --dart-define=API_BASE_URL=<url>.';

  @override
  String get loading => 'جارٍ التحميل';

  @override
  String get secureSessionNote => 'الوصول محمي بجلسة آمنة قصيرة المدة.';

  @override
  String get alreadyHaveAccount => 'لديك حساب بالفعل؟';

  @override
  String get noAccount => 'جديد في عُهدة؟';
}
