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
      'جلسة عُهدة الآمنة وأدوات الشركة المعتمدة جاهزة. الوحدات المالية مؤجلة.';

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
  String get permissionDenied => 'ليست لديك صلاحية لتنفيذ هذا الإجراء.';

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

  @override
  String get navHome => 'الرئيسية';

  @override
  String get navInvitations => 'الدعوات';

  @override
  String get navJoinRequests => 'طلبات الانضمام';

  @override
  String get navAccount => 'الحساب';

  @override
  String welcomeUser(String name) {
    return 'مرحبًا، $name';
  }

  @override
  String get managerOverviewBody =>
      'أدر طرق انضمام الأشخاص إلى شركتك من دون كشف بيانات الحساب الحساسة.';

  @override
  String get memberOverviewBody =>
      'مساحة شركتك الموثقة جاهزة لوحدات المنتج المعتمدة التالية.';

  @override
  String get identityStatus => 'حالة الهوية';

  @override
  String get accessAdministration => 'إدارة الوصول';

  @override
  String get invitationShortcutBody => 'أنشئ دعوات الشركة وراجعها وألغها.';

  @override
  String get joinRequestShortcutBody =>
      'راجع طلبات الانضمام واتخذ القرار بشأنها.';

  @override
  String get financialModulesDeferred =>
      'العُهد والمصروفات والموردون والتسويات والديون والتقارير غير منفذة في هذه المرحلة.';

  @override
  String get accountTitle => 'الحساب والجلسة';

  @override
  String get language => 'لغة التطبيق';

  @override
  String get cancelAction => 'إلغاء';

  @override
  String get finishAction => 'إنهاء';

  @override
  String get refresh => 'تحديث';

  @override
  String get statusFilter => 'تصفية حسب الحالة';

  @override
  String get allStatuses => 'كل الحالات';

  @override
  String get assignedRole => 'الدور المعيّن';

  @override
  String get createdAt => 'تاريخ الإنشاء';

  @override
  String get expiresAt => 'تاريخ الانتهاء';

  @override
  String get acceptedAt => 'تاريخ القبول';

  @override
  String get cancelledAt => 'تاريخ الإلغاء';

  @override
  String get requestedAt => 'تاريخ الطلب';

  @override
  String get reviewedAt => 'تاريخ المراجعة';

  @override
  String get email => 'البريد الإلكتروني';

  @override
  String get requestMessage => 'رسالة الطلب';

  @override
  String get reviewNotes => 'ملاحظات المراجعة';

  @override
  String get reviewNotesOptional => 'ملاحظات المراجعة (اختيارية)';

  @override
  String get rejectionReason => 'سبب الرفض';

  @override
  String get invalidRejectionReason =>
      'استخدم من 1 إلى 500 حرف من دون أسطر جديدة.';

  @override
  String get invitationStatusPending => 'قيد الانتظار';

  @override
  String get invitationStatusAccepted => 'مقبولة';

  @override
  String get invitationStatusExpired => 'منتهية';

  @override
  String get invitationStatusCancelled => 'ملغاة';

  @override
  String get joinStatusPending => 'قيد الانتظار';

  @override
  String get joinStatusApproved => 'مقبول';

  @override
  String get joinStatusRejected => 'مرفوض';

  @override
  String get joinStatusCancelled => 'ملغى';

  @override
  String get createInvitation => 'إنشاء دعوة';

  @override
  String get noInvitationsTitle => 'لا توجد دعوات';

  @override
  String get noInvitationsBody => 'أنشئ دعوة أو غيّر تصفية الحالة.';

  @override
  String get retryLoadingMore => 'إعادة محاولة تحميل المزيد';

  @override
  String get cancelInvitation => 'إلغاء الدعوة';

  @override
  String get cancelInvitationTitle => 'هل تريد إلغاء هذه الدعوة؟';

  @override
  String get cancelInvitationBody =>
      'يمنع الإلغاء قبول هذه الدعوة لاحقًا، وسيظل السجل ظاهرًا.';

  @override
  String get invitationCancelled => 'تم إلغاء الدعوة.';

  @override
  String get invitationCreatedTitle => 'تم إنشاء الدعوة';

  @override
  String get oneTimeTokenWarning =>
      'يظهر هذا الرمز مرة واحدة. انسخه الآن وسلّمه بأمان، إذ لا يمكن استعادته لاحقًا.';

  @override
  String get invitationDeliveryNotImplemented =>
      'إرسال الرسائل النصية والبريد الإلكتروني غير منفذ. شارك الرمز عبر قناة آمنة معتمدة.';

  @override
  String get copyToken => 'نسخ الرمز';

  @override
  String get tokenCopied => 'تم نسخ رمز الدعوة.';

  @override
  String get invitationCreationConflict =>
      'توجد دعوة نشطة أو حساب يتعارض مع هذه البيانات.';

  @override
  String get accessConflictError =>
      'تغيّر هذا السجل على الخادم. حدّث القائمة وحاول مجددًا.';

  @override
  String get invitationNotFound => 'لم تعد الدعوة متاحة.';

  @override
  String get joinRequestNotFound => 'لم يعد طلب الانضمام متاحًا.';

  @override
  String get notFoundError => 'السجل المطلوب غير متاح.';

  @override
  String get noJoinRequestsTitle => 'لا توجد طلبات انضمام';

  @override
  String get noJoinRequestsBody => 'لا توجد طلبات بالحالة المحددة.';

  @override
  String get approve => 'موافقة';

  @override
  String get reject => 'رفض';

  @override
  String get approveJoinRequestTitle => 'الموافقة على طلب الانضمام';

  @override
  String get approveJoinRequestBody =>
      'أكّد الدور النهائي. الدور الذي طلبه المتقدم للمعلومة فقط.';

  @override
  String get sensitiveRoleApprovalNotice =>
      'قد يظل حساب النائب أو المحاسب قيد الانتظار إلى أن يكتمل التحقق من الهوية.';

  @override
  String get standardRoleApprovalNotice =>
      'قد يُفعّل حساب المشرف أو العامل فورًا وفق قواعد الخادم.';

  @override
  String get approvedAndActivated => 'تمت الموافقة على الطلب وتفعيل الحساب.';

  @override
  String get approvedPendingIdentity =>
      'تمت الموافقة على الطلب وهو بانتظار التحقق من الهوية.';

  @override
  String get rejectJoinRequestTitle => 'رفض طلب الانضمام';

  @override
  String get rejectJoinRequestBody =>
      'سيظل السجل المرفوض ظاهرًا ولن يتمكن المتقدم من تسجيل الدخول.';

  @override
  String get rejectionLoginNotice =>
      'يمنع الرفض هذا المتقدم من تسجيل الدخول. اكتب سببًا واضحًا.';

  @override
  String get joinRequestRejected => 'تم رفض طلب الانضمام.';

  @override
  String get navProjects => 'المشاريع';

  @override
  String get navCompanyMembers => 'أعضاء الشركة';

  @override
  String get projectsTitle => 'المشاريع والمواقع';

  @override
  String get companyMembersTitle => 'دليل أعضاء الشركة';

  @override
  String get searchProjects => 'ابحث باسم المشروع أو عنوان الموقع';

  @override
  String get searchMembers => 'ابحث بالاسم أو الهاتف';

  @override
  String get searchMinimum => 'أدخل حرفين على الأقل للبحث.';

  @override
  String get roleFilter => 'تصفية حسب الدور';

  @override
  String get allRoles => 'كل الأدوار';

  @override
  String get projectStatusActive => 'نشط';

  @override
  String get projectStatusPaused => 'متوقف مؤقتًا';

  @override
  String get projectStatusCompleted => 'مكتمل';

  @override
  String get projectStatusFinanciallyClosed => 'مغلق ماليًا';

  @override
  String get projectStatusCancelled => 'ملغى';

  @override
  String get projectName => 'اسم المشروع';

  @override
  String get siteAddress => 'عنوان الموقع';

  @override
  String get ownerClient => 'مالك المشروع / العميل';

  @override
  String get ownerName => 'اسم المالك';

  @override
  String get contractValue => 'قيمة العقد';

  @override
  String get contractValueLyd => 'قيمة العقد (د.ل)';

  @override
  String get contractDate => 'تاريخ العقد';

  @override
  String get startDate => 'تاريخ البدء';

  @override
  String get expectedEndDate => 'تاريخ الانتهاء المتوقع';

  @override
  String get actualEndDate => 'تاريخ الانتهاء الفعلي';

  @override
  String get descriptionOptional => 'الوصف (اختياري)';

  @override
  String get notesOptional => 'ملاحظات (اختيارية)';

  @override
  String get contactPhoneOptional => 'هاتف التواصل للمشروع (اختياري)';

  @override
  String get addressOptional => 'العنوان (اختياري)';

  @override
  String get assignedSupervisor => 'المشرف المعيّن';

  @override
  String get assignedSupervisors => 'المشرفون المعيّنون';

  @override
  String get createProject => 'إنشاء مشروع';

  @override
  String get editProject => 'تعديل المشروع';

  @override
  String get assignSupervisor => 'تعيين مشرف';

  @override
  String get replaceSupervisor => 'استبدال المشرف';

  @override
  String get projectDetails => 'تفاصيل المشروع';

  @override
  String get projectMembersTitle => 'مشرفو المشروع';

  @override
  String get activeSupervisionAssignmentsBody =>
      'تعرض هذه الصفحة تعيينات الإشراف النشطة فقط، وليست دليلًا كاملًا للعاملين في المشروع.';

  @override
  String get noProjectsTitle => 'لا توجد مشاريع';

  @override
  String get noProjectsBody => 'غيّر عوامل التصفية أو أنشئ أول مشروع.';

  @override
  String get noAssignedProjectsBody =>
      'تظهر هنا فقط المشاريع المعيّنة لك حاليًا.';

  @override
  String get noMembersTitle => 'لا يوجد أعضاء شركة';

  @override
  String get noMembersBody => 'غيّر تصفية الدور أو الحالة أو البحث.';

  @override
  String get memberDetails => 'تفاصيل العضو';

  @override
  String get directoryReadOnly => 'دليل أعضاء الشركة للعرض فقط.';

  @override
  String get projectCreated => 'تم إنشاء المشروع.';

  @override
  String get projectUpdated => 'تم تحديث المشروع.';

  @override
  String get supervisorAssigned => 'تم تحديث تعيين المشرف.';

  @override
  String get projectChangedConflict =>
      'تغيّر هذا المشروع على الخادم. أعد تحميله قبل المحاولة مجددًا.';

  @override
  String get reloadProject => 'إعادة تحميل المشروع';

  @override
  String get newOwnerTitle => 'عميل / مالك جديد';

  @override
  String get existingOwnerUnavailable =>
      'لا يتوفر حاليًا دليل آمن للملاك. ينشئ هذا النموذج المالك والمشروع في معاملة واحدة.';

  @override
  String get chooseSupervisor => 'اختيار مشرف';

  @override
  String get optionalSupervisor => 'المشرف الأولي (اختياري)';

  @override
  String get noSupervisor => 'لا يوجد مشرف نشط';

  @override
  String get replaceSupervisorTitle => 'هل تريد استبدال المشرف النشط؟';

  @override
  String get replaceSupervisorBody =>
      'سينتهي التعيين السابق ولن يُحذف. يحتفظ الخادم بسجل التعيينات.';

  @override
  String get supervisorHistoryNotice =>
      'يحافظ استبدال المشرف على سجل التعيينات السابقة في الخادم.';

  @override
  String get saveAction => 'حفظ';

  @override
  String get selectDate => 'اختيار التاريخ';

  @override
  String get clearAction => 'مسح';

  @override
  String get viewDetails => 'عرض التفاصيل';

  @override
  String get invalidRequiredText => 'أدخل قيمة غير فارغة ضمن الطول المسموح.';

  @override
  String get invalidContractValue =>
      'أدخل قيمة موجبة بمنزلتين عشريتين كحد أقصى وبحد أقصى 16 رقمًا صحيحًا.';

  @override
  String get invalidDateOrder =>
      'لا يمكن أن يسبق تاريخ الانتهاء المتوقع تاريخ البدء.';

  @override
  String get supervisorPickerEmpty => 'لا يوجد مشرفون نشطون يطابقون البحث.';

  @override
  String get updatedAt => 'تاريخ التحديث';

  @override
  String get assignedAt => 'تاريخ التعيين';

  @override
  String get readOnly => 'للعرض فقط';
}
