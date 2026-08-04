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
      'جلسة عُهدة الآمنة وأدوات الشركة والعُهد وأرصدة الحيازة المعتمدة جاهزة.';

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
      'تبقى المصروفات والإيصالات والموردون والتسويات والإغلاق والديون والتقارير مؤجلة.';

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

  @override
  String get navAdvances => 'العُهد';

  @override
  String get advancesTitle => 'العُهد';

  @override
  String get advanceDetails => 'تفاصيل العُهدة';

  @override
  String get movementHistory => 'سجل الحركات';

  @override
  String get personalBalance => 'أرصدة عُهدي';

  @override
  String get userBalances => 'أرصدة المستخدم';

  @override
  String get fundingSources => 'مصادر التمويل';

  @override
  String get createAdvance => 'إنشاء عُهدة';

  @override
  String get distributeMoney => 'توزيع مبلغ';

  @override
  String get confirmReceipt => 'تأكيد الاستلام';

  @override
  String get rejectTransfer => 'رفض التحويل';

  @override
  String get returnUnusedMoney => 'إرجاع المبلغ غير المستخدم';

  @override
  String get originalAmount => 'المبلغ الأصلي';

  @override
  String get availableAmount => 'المبلغ المتاح';

  @override
  String get reservedAmount => 'المبلغ المحجوز';

  @override
  String get currency => 'العملة';

  @override
  String get fundingSource => 'مصدر التمويل';

  @override
  String get fundingAllocation => 'تخصيص التمويل';

  @override
  String get recipient => 'المستلم';

  @override
  String get sender => 'المرسل';

  @override
  String get status => 'الحالة';

  @override
  String get transferType => 'نوع التحويل';

  @override
  String get paymentMethod => 'طريقة الدفع';

  @override
  String get pendingConfirmation => 'بانتظار التأكيد';

  @override
  String get confirmed => 'مؤكد';

  @override
  String get rejected => 'مرفوض';

  @override
  String get retrySameOperation => 'إعادة نفس العملية';

  @override
  String get operationStatusUncertain => 'حالة العملية غير مؤكدة';

  @override
  String get idempotentRetryExplanation =>
      'ربما وصل الطلب إلى الخادم. أعد المحاولة للعملية نفسها فقط؛ ستعيد عُهدة استخدام هوية العملية المحمية.';

  @override
  String get insufficientAvailableBalance =>
      'المبلغ أكبر من الرصيد المتاح المعروض حاليًا.';

  @override
  String get staleFinancialState =>
      'تغيرت الحالة المالية. أعد التحميل قبل المحاولة.';

  @override
  String get balanceChangedReload => 'تغيّر الرصيد؛ أعد التحميل';

  @override
  String get returnDestinationDerived =>
      'تُشتق وجهة الإرجاع تلقائيًا من سجل التحويلات السابقة المؤكدة.';

  @override
  String get expensesNotImplemented => 'المصروفات غير منفذة في هذه المرحلة.';

  @override
  String get settlementNotImplemented =>
      'التسوية والإغلاق غير منفذين في هذه المرحلة.';

  @override
  String get noAdvancesTitle => 'لا توجد عُهد';

  @override
  String get noAdvancesBody => 'غيّر عوامل التصفية المعتمدة أو أنشئ أول عُهدة.';

  @override
  String get participantAdvancesBody => 'تظهر هنا فقط العُهد التي شاركت فيها.';

  @override
  String get searchAdvanceReference => 'البحث بمرجع العُهدة';

  @override
  String get advanceReference => 'مرجع العُهدة';

  @override
  String get purpose => 'الغرض';

  @override
  String get issueDate => 'تاريخ الإصدار';

  @override
  String get settlementDueDateOptional => 'موعد التسوية (اختياري)';

  @override
  String get transferDate => 'تاريخ التحويل';

  @override
  String get bankName => 'اسم المصرف';

  @override
  String get referenceNumber => 'رقم المرجع';

  @override
  String get description => 'الوصف';

  @override
  String get amount => 'المبلغ';

  @override
  String get invalidMoney =>
      'أدخل مبلغًا موجبًا بحد أقصى 16 رقمًا صحيحًا ومنزلتين عشريتين.';

  @override
  String get fundingTotalMismatch =>
      'يجب أن تساوي تخصيصات التمويل مبلغ العُهدة تمامًا.';

  @override
  String get selectRecipient => 'اختيار المستلم';

  @override
  String get noEligibleRecipients => 'لم يُعثر على مستلمين نشطين مؤهلين.';

  @override
  String get noFundingSources => 'لم يُرجع الخادم مصادر تمويل قابلة للاستخدام.';

  @override
  String get advanceCreatedPending =>
      'تم إنشاء العُهدة وهي بانتظار تأكيد المستلم.';

  @override
  String get distributionPending => 'تم حجز التوزيع وهو بانتظار تأكيد المستلم.';

  @override
  String get returnPending => 'تم حجز الإرجاع وهو بانتظار تأكيد المستلم.';

  @override
  String get confirmFinancialOperationTitle => 'هل تؤكد هذه العملية المالية؟';

  @override
  String get confirmReceiptBody =>
      'أكد فقط إذا استلمت هذا المبلغ نفسه. سيطبق الخادم أثر الرصيد المعتمد.';

  @override
  String get rejectTransferBody =>
      'يحافظ الرفض على سجل التحويل ويحرر حجز المرسل وفق قواعد الخادم.';

  @override
  String get distributionEffectBody =>
      'سيُحجز المبلغ حتى يؤكده المستلم أو يرفضه.';

  @override
  String get cancelUncertainOperation => 'إلغاء إعادة المحاولة المحلية';

  @override
  String get noMovementsTitle => 'لا توجد حركات مسجلة';

  @override
  String get noMovementsBody => 'لم يُرجع الخادم حركات لهذه العُهدة.';

  @override
  String get noBalancesTitle => 'لا توجد أرصدة عُهد';

  @override
  String get noBalancesBody => 'لم يُرجع الخادم أرصدة حيازة معتمدة.';

  @override
  String get pendingReservationsNotice =>
      'تبقى المبالغ المحجوزة غير متاحة ما دام التحويل المدعوم قيد الانتظار.';

  @override
  String get authorizedBalanceLookup => 'عرض رصيد مستخدم';

  @override
  String get accountantBalanceLookupLimitation =>
      'تتطلب الواجهة البرمجية معرّف مستخدم، ولا يملك المحاسب دليل شركة آمنًا للاختيار. تبقى الأرصدة الشخصية متاحة.';

  @override
  String get balanceHolder => 'حائز الرصيد';

  @override
  String get receivedAmount => 'المستلم';

  @override
  String get restoredAmount => 'المستعاد';

  @override
  String get transferredAmount => 'المحوّل للخارج';

  @override
  String get returnedAmount => 'المُرجع';

  @override
  String get fundingSourceTypeProjectOwnerPayment => 'دفعة مالك المشروع';

  @override
  String get fundingSourceTypeManagerContribution => 'مساهمة المدير';

  @override
  String get fundingSourceTypeCompanyCashbox => 'صندوق الشركة';

  @override
  String get fundingSourceTypeReturnedAdvance => 'عُهدة مُرجعة';

  @override
  String get fundingSourceTypeSupplierRefund => 'استرداد من مورد';

  @override
  String get other => 'أخرى';

  @override
  String get paymentCash => 'نقدي';

  @override
  String get paymentBankTransfer => 'تحويل مصرفي';

  @override
  String get paymentCheque => 'صك';

  @override
  String get paymentCard => 'بطاقة';

  @override
  String get paymentMobileWallet => 'محفظة إلكترونية';

  @override
  String get paymentBalanceTransfer => 'تحويل رصيد';

  @override
  String get advanceStatusDraft => 'مسودة';

  @override
  String get advanceStatusOpen => 'مفتوحة';

  @override
  String get advanceStatusInSettlement => 'قيد التسوية';

  @override
  String get advanceStatusReadyToClose => 'جاهزة للإغلاق';

  @override
  String get advanceStatusClosed => 'مغلقة';

  @override
  String get advanceStatusCancelled => 'ملغاة';

  @override
  String get advanceStatusReversed => 'معكوسة';

  @override
  String get transferStatusCorrectionRequired => 'تحتاج تصحيحًا';

  @override
  String get transferTypeAdvanceDelivery => 'تسليم العُهدة الأولي';

  @override
  String get transferTypeInternalTransfer => 'توزيع داخلي';

  @override
  String get transferTypeBalanceReturn => 'إرجاع رصيد';

  @override
  String get movementFundingAllocation => 'تخصيص تمويل';

  @override
  String get fundingStatusAvailable => 'متاح';

  @override
  String get fundingStatusPartiallyUsed => 'مستخدم جزئيًا';

  @override
  String get fundingStatusFullyUsed => 'مستخدم بالكامل';

  @override
  String get fundingStatusPendingVerification => 'بانتظار التحقق';

  @override
  String get balanceStatusInSettlement => 'قيد التسوية';

  @override
  String get balanceStatusSettled => 'مسوّى';

  @override
  String get balanceStatusClosed => 'مغلق';

  @override
  String get selectUser => 'اختيار مستخدم';

  @override
  String get viewMovements => 'عرض الحركات';

  @override
  String get financialOperationSucceeded => 'اكتملت العملية المالية بنجاح.';

  @override
  String get navExpenses => 'المصروفات';

  @override
  String get expensesTitle => 'المصروفات';

  @override
  String get expenseDetails => 'تفاصيل المصروف';

  @override
  String get createExpense => 'إنشاء مصروف';

  @override
  String get expenseCategories => 'فئات المصروفات';

  @override
  String get createExpenseCategory => 'إنشاء فئة مصروفات';

  @override
  String get reimbursements => 'الاستردادات الشخصية';

  @override
  String get reimbursementDetails => 'تفاصيل الاسترداد';

  @override
  String get expenseHistory => 'سجل المصروف';

  @override
  String get documentMetadata => 'بيانات المستند';

  @override
  String get binaryUploadNotImplemented =>
      'رفع الملفات غير منفذ في هذه المرحلة. هذه سجلات بيانات وصفية للمستندات فقط.';

  @override
  String get paymentMode => 'طريقة التمويل';

  @override
  String get advanceBalance => 'رصيد عُهدة';

  @override
  String get personalFunds => 'أموال شخصية';

  @override
  String get personalFundsClaimNotice =>
      'سيُنشأ طلب استرداد كالتزام غير مدفوع.';

  @override
  String get advanceReservationNotice =>
      'يُحجز التخصيص حتى المراجعة؛ الاعتماد يؤكده والرفض يحرره.';

  @override
  String get expenseReference => 'مرجع المصروف';

  @override
  String get searchExpenseReference => 'البحث بمرجع المصروف';

  @override
  String get expenseDate => 'تاريخ المصروف';

  @override
  String get category => 'الفئة';

  @override
  String get categoryName => 'اسم الفئة';

  @override
  String get categoryCodeOptional => 'رمز الفئة (اختياري)';

  @override
  String get categoryGroup => 'مجموعة الفئة';

  @override
  String get categoryScope => 'نطاق الفئة';

  @override
  String get requiresReceipt => 'يتطلب إيصالًا داعمًا';

  @override
  String get requiresSupplier => 'يتطلب موردًا';

  @override
  String get supportsQuantityDetails => 'يدعم تفاصيل الكميات';

  @override
  String get displayOrder => 'ترتيب العرض';

  @override
  String get receiptNumberOptional => 'رقم الإيصال (اختياري)';

  @override
  String get invoiceNumberOptional => 'رقم الفاتورة (اختياري)';

  @override
  String get merchantNameOptional => 'اسم التاجر (اختياري)';

  @override
  String get expenseLocationOptional => 'موقع المصروف (اختياري)';

  @override
  String get projectOptional => 'المشروع (اختياري)';

  @override
  String get companyExpense => 'مصروف شركة';

  @override
  String get projectExpense => 'مصروف مشروع';

  @override
  String get incurredBy => 'الدافع';

  @override
  String get submittedBy => 'مقدم الطلب';

  @override
  String get reviewedBy => 'المراجع';

  @override
  String get reviewExpense => 'مراجعة المصروف';

  @override
  String get approveExpense => 'اعتماد المصروف';

  @override
  String get rejectExpense => 'رفض المصروف';

  @override
  String get approveExpenseNotice =>
      'الاعتماد يؤكد حجوزات أرصدة العُهد. تظل المطالبات الشخصية التزامات غير مدفوعة.';

  @override
  String get rejectExpenseNotice =>
      'الرفض يحرر حجوزات العُهدة وقد يلغي مطالبة لم تُمس. يبقى المصروف في السجل.';

  @override
  String get claimUnpaid => 'المطالبة غير مدفوعة';

  @override
  String get outstandingAmount => 'المبلغ المتبقي';

  @override
  String get claimant => 'صاحب المطالبة';

  @override
  String get noExpensesTitle => 'لا توجد مصروفات';

  @override
  String get noExpensesBody => 'غيّر عوامل التصفية المعتمدة أو أنشئ أول مصروف.';

  @override
  String get supervisorExpensesBody =>
      'تظهر هنا مصروفاتك ومصروفات المشاريع المسندة إليك بنشاط فقط.';

  @override
  String get workerExpensesBody => 'تظهر هنا مصروفاتك الشخصية فقط.';

  @override
  String get noReimbursements => 'لم تُرجع الخوادم مطالبات استرداد.';

  @override
  String get noCategories => 'لم تُرجع الخوادم فئات مصروفات نشطة.';

  @override
  String get noDocuments => 'لا توجد سجلات بيانات وصفية للمستندات.';

  @override
  String get noExpenseHistory => 'لا توجد أحداث في سجل المصروف.';

  @override
  String get allocationTotalMismatch =>
      'يجب أن تساوي تخصيصات العُهدة مبلغ المصروف تمامًا.';

  @override
  String get selectAdvanceBalance => 'اختيار رصيد عُهدة';

  @override
  String get supportingDocumentRequired =>
      'قد يلزم سجل بيانات وصفية لإيصال أو فاتورة قبل الاعتماد.';

  @override
  String get expenseCreatedPending => 'تم إنشاء المصروف وهو بانتظار المراجعة.';

  @override
  String get expenseStatusPendingReview => 'بانتظار المراجعة';

  @override
  String get expenseStatusCorrectionRequired => 'يتطلب تصحيحًا';

  @override
  String get expenseStatusApproved => 'معتمد';

  @override
  String get expenseStatusRejected => 'مرفوض';

  @override
  String get expenseStatusCancelled => 'ملغى';

  @override
  String get expenseStatusReversed => 'معكوس';

  @override
  String get categoryGroupMaterials => 'مواد';

  @override
  String get categoryGroupLabor => 'عمالة';

  @override
  String get categoryGroupSubcontracting => 'مقاولات باطنة';

  @override
  String get categoryGroupTransportation => 'نقل';

  @override
  String get categoryGroupEquipment => 'معدات';

  @override
  String get categoryGroupFuel => 'وقود';

  @override
  String get categoryGroupServices => 'خدمات';

  @override
  String get categoryGroupAdministrative => 'إداري';

  @override
  String get categoryGroupUtilities => 'مرافق';

  @override
  String get categoryGroupPermits => 'تصاريح';

  @override
  String get scopeProjectOnly => 'مشروع فقط';

  @override
  String get scopeCompanyOnly => 'شركة فقط';

  @override
  String get scopeBoth => 'مشروع أو شركة';

  @override
  String get documentTypeReceipt => 'إيصال';

  @override
  String get documentTypeInvoice => 'فاتورة';

  @override
  String get documentTypeQuotation => 'عرض سعر';

  @override
  String get documentTypeDeliveryNote => 'إذن تسليم';

  @override
  String get documentTypePaymentProof => 'إثبات دفع';

  @override
  String get documentTypeContract => 'عقد';

  @override
  String get documentTypePurchaseOrder => 'أمر شراء';

  @override
  String get documentStatusPendingVerification => 'بانتظار التحقق';

  @override
  String get documentStatusVerified => 'متحقق منه';

  @override
  String get claimStatusOpen => 'مفتوحة';

  @override
  String get claimStatusPartiallySettled => 'مسواة جزئيًا';

  @override
  String get claimStatusSettled => 'مسواة';

  @override
  String get claimStatusCancelled => 'ملغاة';

  @override
  String get claimStatusReversed => 'معكوسة';

  @override
  String get receiptNumber => 'رقم الإيصال';

  @override
  String get invoiceNumber => 'رقم الفاتورة';

  @override
  String get advanceAllocations => 'تخصيصات العُهدة';

  @override
  String get expenseItems => 'بنود المصروف';

  @override
  String get documents => 'المستندات';

  @override
  String get history => 'السجل';

  @override
  String get fileName => 'اسم الملف';

  @override
  String get fileSize => 'حجم الملف';

  @override
  String get verificationStatus => 'حالة التحقق';

  @override
  String get claimDate => 'تاريخ المطالبة';

  @override
  String get dueDate => 'تاريخ الاستحقاق';

  @override
  String get categoryCreated => 'تم إنشاء فئة المصروفات.';

  @override
  String get expenseApproved => 'تم اعتماد المصروف.';

  @override
  String get expenseRejected => 'تم رفض المصروف.';

  @override
  String get historyExpenseCreated => 'تم تقديم المصروف للمراجعة';

  @override
  String get historyDocumentAdded => 'تمت إضافة بيانات المستند الداعم';

  @override
  String get historyExpenseApproved => 'تم اعتماد المصروف';

  @override
  String get historyExpenseRejected => 'تم رفض المصروف';

  @override
  String get historyOutcomeSuccess => 'نجحت';
}
