package com.brierley.loyaltyware.clientlib.domainmodel;

import java.util.*;

import com.brierley.loyaltyware.clientlib.LWClientException;

public class LWAttributeSetContainer {
	private List<LWAttributeSetContainer> children = new LinkedList<LWAttributeSetContainer>();
	private Hashtable<String,Object> transientProperties = new Hashtable<String, Object>();


	public void addTransientProperty(String propertyName, Object propertyValue)
    {
        transientProperties.put(propertyName.toLowerCase(), propertyValue);
    }

	public void updateTransientProperty(String propertyName, Object propertyValue)
    {
        if (transientProperties.containsKey(propertyName.toLowerCase()))
        {
            transientProperties.remove(propertyName.toLowerCase());
        }
        transientProperties.put(propertyName.toLowerCase(), propertyValue);
    }

	public void removeTransientProperty(String propertyName)
    {
        if (transientProperties.containsKey(propertyName.toLowerCase()))
        {
            transientProperties.remove(propertyName.toLowerCase());
        }       
    }

	public Object getTransientProperty(String propertyName)
    {
        return transientProperties.get(propertyName.toLowerCase());
    }

    public Boolean hasTransientProperty(String propertyName)
    {
        return transientProperties.containsKey(propertyName.toLowerCase());
    }

	public Enumeration<String> getTransientPropertyNames()
	{
	    return transientProperties.keys();
    }

    public List<LWAttributeSetContainer> getChildren() {
		return children;
	}

	public void add(LWAttributeSetContainer attSet) throws LWClientException{
        try
        {
            children.add(attSet);
        }
        catch (Exception ex)
        {
            throw new LWClientException("Error adding attribute set.", ex, 1);
        }
    }

    public List<LWAttributeSetContainer> getAttributeSets(String name) {
			List<LWAttributeSetContainer> list = new ArrayList<LWAttributeSetContainer>();
			for (LWAttributeSetContainer obj : children) {
				String objClassName = obj.getClass().getName();
				if (obj instanceof LWClientDataObject && objClassName.endsWith(name)) {
					list.add(obj);
				}
			}
			return list;
	}
}
