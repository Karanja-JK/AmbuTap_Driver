package crc646f3ae7d5bbbf8e50;


public class NotificationHelper
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("AmbuTap_Driver.Helpers.NotificationHelper, AmbuTap Driver", NotificationHelper.class, __md_methods);
	}


	public NotificationHelper ()
	{
		super ();
		if (getClass () == NotificationHelper.class)
			mono.android.TypeManager.Activate ("AmbuTap_Driver.Helpers.NotificationHelper, AmbuTap Driver", "", this, new java.lang.Object[] {  });
	}

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
