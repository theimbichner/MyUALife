package md5b26a268dc2aa83d050a79129cfdc8f4a;


public class DateManager
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.widget.CalendarView.OnDateChangeListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onSelectedDayChange:(Landroid/widget/CalendarView;III)V:GetOnSelectedDayChange_Landroid_widget_CalendarView_IIIHandler:Android.Widget.CalendarView/IOnDateChangeListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("MyUALife.DateManager, MyUALife, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DateManager.class, __md_methods);
	}


	public DateManager () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DateManager.class)
			mono.android.TypeManager.Activate ("MyUALife.DateManager, MyUALife, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onSelectedDayChange (android.widget.CalendarView p0, int p1, int p2, int p3)
	{
		n_onSelectedDayChange (p0, p1, p2, p3);
	}

	private native void n_onSelectedDayChange (android.widget.CalendarView p0, int p1, int p2, int p3);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
