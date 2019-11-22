package com.brierley.loyaltyware.clientlib;

import java.text.*;
import java.util.*;
import java.lang.reflect.*;

import javax.xml.xpath.XPath;
import javax.xml.xpath.XPathConstants;
import javax.xml.xpath.XPathExpressionException;
import javax.xml.xpath.XPathFactory;

import org.w3c.dom.*;

import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;
import org.joda.time.format.DateTimeFormatter;
import org.joda.time.format.ISODateTimeFormat;

import com.brierley.loyaltyware.clientlib.annotations.*;
import com.brierley.loyaltyware.clientlib.domainmodel.*;
import com.brierley.loyaltyware.clientlib.domainmodel.framework.Member;
import com.brierley.loyaltyware.clientlib.domainmodel.framework.VirtualCard;

public class LWXmlSerializer {
	public LWXmlSerializer() {
	}

	private Date convertStringToDate(String dateStr) {
		// ISO 8601
		DateTimeFormatter formatter = ISODateTimeFormat.dateTimeParser();
		DateTime dt = formatter.parseDateTime(dateStr);
		Date result = dt.toDate();
		return result;
	}

	private String convertDateToString(Date dt) {
		// ISO 8601
		DateTimeFormatter formatter = ISODateTimeFormat.dateTime();
		String result = formatter.print(new DateTime(dt));
		return result;
	}

	private Boolean isBrowsable(Field field) {
		return true;
	}

	private Boolean isRequired(Field field) {
		Boolean result = field.getAnnotation(LWIsRequired.class) != null;
		return result;
	}

	private int getStringLength(Field field) {
		int result = Integer.MAX_VALUE;
		LWStringLength stringLength = field.getAnnotation(LWStringLength.class);
		if (stringLength != null && stringLength.value() > 0) {
			result = stringLength.value();
		}
        return result;
    }

	private void setFieldValue(LWAttributeSetContainer thisObject, Field field, Object value) throws SecurityException, NoSuchMethodException, IllegalArgumentException,
			IllegalAccessException, InvocationTargetException {
		String methodName = "set" + LWUtils.capitalize(field.getName());
		Class<?> cl = field.getType();
		Method setter = thisObject.getClass().getDeclaredMethod(methodName, cl);
		Object parms[] = { value };
		setter.invoke(thisObject, parms);
	}

	private Element serializeObjectToXml(Document doc, Element thisNode, LWAttributeSetContainer thisObject) throws LWClientException {
		Class<?> c = thisObject.getClass();
		for (Field field : c.getDeclaredFields()) {
			if (!isBrowsable(field)) {
				continue;
			}
			String attValue = null;
			Object val = null;
			try {
				field.setAccessible(true);
				val = field.get(thisObject);
			} catch (IllegalArgumentException e) {
				throw new LWClientException("Error getting value of field " + field.getName(), e, 10001);
			} catch (IllegalAccessException e) {
				throw new LWClientException("Error getting value of field " + field.getName(), e, 10001);
			}
			String fieldName = field.getName();
			if (val != null) {
				if (val instanceof Date) {
					attValue = convertDateToString((Date) val);
				} else if (val instanceof Long) {
					attValue = Long.toString((Long) val);
				} else if (val instanceof java.math.BigDecimal) {
					attValue = val.toString();
				} else if (val.getClass().isEnum()) {
					attValue = val.toString();
				} else if (val instanceof String) {
					attValue = val.toString();
					int stringLength = getStringLength(field);
					if (attValue != null && !"".equals(attValue) && stringLength > 0 && attValue.length() > stringLength) {
						String msg = "Attribute " + fieldName + " of " + thisObject.getClass().getSimpleName() + " cannot be more than " + stringLength + " characters.";
                        throw new LWClientException(msg, 2002);
                    }
				} else {
					attValue = val.toString();
				}
			}
			if (thisObject instanceof LWAttributeSetContainer)
				fieldName = LWUtils.capitalize(fieldName);
			if (LWUtils.isEmptyString(attValue) && isRequired(field)) {
				String errorMessage = String.format("%s of %s is a required property.  Please provide a valid value.", fieldName, thisObject.getClass().getName());
				throw new LWClientException(errorMessage, 2003);
			} else {
				if (attValue != null)
					thisNode.setAttribute(fieldName, attValue);
			}
			/*
			 * if (!LWUtils.isEmptyString(attValue)) {
			 * thisNode.setAttribute(fieldName, attValue); } else if
			 * (isRequired(field)) { String errorMessage = String .format(
			 * "%s of %s is a required property.  Please provide a valid value."
			 * , fieldName, thisObject.getClass() .getName()); throw new
			 * LWClientException(errorMessage, 2003); }
			 */
		}
		Enumeration<String> titer = thisObject.getTransientPropertyNames();
				while(titer.hasMoreElements()){
					String name = titer.nextElement();
					Object value = thisObject.getTransientProperty(name);
					thisNode.setAttribute(name, value.toString());
		}
		Iterator<LWAttributeSetContainer> iter = thisObject.getChildren().iterator();
		while (iter.hasNext()) {
			LWAttributeSetContainer child = iter.next();
			Element childNode = doc.createElement(child.getClass().getSimpleName());
			childNode = serializeObjectToXml(doc, childNode, child);
			thisNode.appendChild(childNode);
		}
		return thisNode;
	}

	public String serializeMember(Member member) throws LWClientException {
		Document doc = LWUtils.getDocument();
		Element root = doc.createElement("AttributeSets");
		Element memberNode = doc.createElement("Member");
		root.appendChild(memberNode);
		doc.appendChild(root);
		memberNode = serializeObjectToXml(doc, memberNode, member);
		return LWUtils.getXml(doc);
	}

	public String serializeDataObject(String opName, String paramName, String parm) {
		return String.format("<%sIn %s=\"%s\"/>", opName, paramName, parm);
	}

	public String serializeDataObject(String opName, String paramName, int parm) {
		return String.format("<%sIn %s=\"%d\"/>", opName, paramName, parm);
	}

	public String serializeDataObject(String opName, String paramName, long parm) {
		return String.format("<%sIn %s=\"%d\"/>", opName, paramName, parm);
	}

	public String serializeDataObject(String opName, String paramName, java.math.BigDecimal parm) {
		return String.format("<%sIn %s=\"%f\"/>", opName, paramName, parm);
	}

	public String serializeDataObject(String opName, String paramName, Boolean parm) {
		return String.format("<%sIn %s=\"%s\"/>", opName, paramName, parm.toString());
	}

	public String serializeDataObject(String opName, String paramName, Date parm) {
		return String.format("<%sIn %s=\"%s\"/>", opName, paramName, convertDateToString(parm));
	}

	public String serializeDataObject(Object thisObject) throws LWClientException {
		Document doc = LWUtils.getDocument();
		Element root = doc.createElement(thisObject.getClass().getSimpleName());
		doc.appendChild(root);

		Class<?> cl = thisObject.getClass();
		for (Field field : cl.getDeclaredFields()) {
			String attValue = null;
			Object val = null;
			try {
				field.setAccessible(true);
				val = field.get(thisObject);
			} catch (IllegalArgumentException e) {
				throw new LWClientException("Error getting value of field " + field.getName(), e, 10001);
			} catch (IllegalAccessException e) {
				throw new LWClientException("Error getting value of field " + field.getName(), e, 10001);
			}
			if (val != null) {
				if (val.getClass() == Date.class) {
					attValue = convertDateToString((Date) val);
				} else if (val.getClass() == Long.class) {
					attValue = Long.toString((Long) val);
				} else if (val.getClass().isEnum()) {
					attValue = val.toString();
				} else {
					attValue = val.toString();
				}
			}
			if (!LWUtils.isEmptyString(attValue)) {
				root.setAttribute(field.getName(), attValue);
			} else if (isRequired(field)) {
				String errorMessage = String.format("%s of %s is a required property.  Please provide a valid value.", field.getName(), thisObject.getClass().getName());
				throw new LWClientException(errorMessage, 1);
			}
		}

		return LWUtils.getXml(doc);
	}

	public String serializeDataObjects(String methodName, List<?> objects) throws LWClientException {
		Document doc = LWUtils.getDocument();
		Element root = doc.createElement(methodName + "InParms");
		doc.appendChild(root);
		root.setAttribute("IsArray", "true");
		for (Object thisObject : objects) {
			Class<?> cl = thisObject.getClass();
			Element node = doc.createElement(cl.getSimpleName());
			root.appendChild(node);
			for (Field field : cl.getDeclaredFields()) {
				String attValue = null;
				Object val = null;
				try {
					field.setAccessible(true);
					val = field.get(thisObject);
				} catch (IllegalArgumentException e) {
					throw new LWClientException("Error getting value of field " + field.getName(), e, 10001);
				} catch (IllegalAccessException e) {
					throw new LWClientException("Error getting value of field " + field.getName(), e, 10001);
				}
				if (val != null) {
					if (val.getClass() == Date.class) {
						attValue = convertDateToString((Date) val);
					} else if (val.getClass() == Long.class) {
						attValue = Long.toString((Long) val);
					} else if (val.getClass().isEnum()) {
						attValue = val.toString();
					} else {
						attValue = val.toString();
					}
				}
				if (!LWUtils.isEmptyString(attValue)) {
					node.setAttribute(field.getName(), attValue);
				} else if (isRequired(field)) {
					String errorMessage = String.format("%s of %s is a required property.  Please provide a valid value.", field.getName(), thisObject.getClass().getName());
					throw new LWClientException(errorMessage, 1);
				}
			}
		}
		return LWUtils.getXml(doc);
	}

	public String serializeGlobalAttributeSet(LWAttributeSetContainer global) throws LWClientException {
		Document doc = LWUtils.getDocument();
		Element root = doc.createElement("AttributeSets");
		Element globalNode = doc.createElement("Global");
		root.appendChild(globalNode);
		doc.appendChild(root);
		Element asetNode = doc.createElement(global.getClass().getSimpleName());
		globalNode.appendChild(asetNode);
		asetNode = serializeObjectToXml(doc, asetNode, global);
		return LWUtils.getXml(doc);
	}

	private LWAttributeSetContainer deserializeObjectFromXml(Element thisNode, LWAttributeSetContainer thisObject) throws LWClientException {
		Hashtable<String, String> processedAttributes = new Hashtable<String, String>();
		Class<?> c = thisObject.getClass();
		String xml = LWUtils.getXml(thisNode);
		//System.out.println(xml);
		for (Field field : c.getDeclaredFields()) {
			String fieldName = LWUtils.capitalize(field.getName());
			processedAttributes.put(fieldName, fieldName);
			Object value = null;
			if (!isBrowsable(field)) {
				continue;
			}
			String attValue = getAttributeValue(thisNode, fieldName, ""); // thisNode.getAttribute(fieldName);
			if (!LWUtils.isEmptyString(attValue)) {
				if (field.getType().equals(Date.class)) {
					value = convertStringToDate(attValue);
				} else if (field.getType().equals(Long.class) || field.getType().equals(long.class)) {
					value = Long.parseLong(attValue);
				} else if (field.getType().equals(Integer.class) || field.getType().equals(int.class)) {
					value = Integer.parseInt(attValue);
				} else if (field.getType().equals(Boolean.class)) {
					value = Boolean.parseBoolean(attValue);
				} else if (field.getType().equals(java.math.BigDecimal.class) || field.getType().equals(java.math.BigDecimal.class)) {
					value = new java.math.BigDecimal(attValue);
				} else if (field.getType().isEnum()) {
					value = Enum.valueOf((Class<Enum>) field.getType(), attValue);
				} else {
					value = attValue;
				}

				try {
					try {
						setFieldValue(thisObject, field, value);
					} catch (SecurityException e) {
						e.printStackTrace();
					} catch (NoSuchMethodException e) {
						e.printStackTrace();
					} catch (InvocationTargetException e) {
						e.printStackTrace();
					}
				} catch (IllegalArgumentException e) {
					throw new LWClientException("Error setting value of field " + field.getName(), e, 10002);
				} catch (IllegalAccessException e) {
					throw new LWClientException("Error setting value of field " + field.getName(), e, 10002);
				}
			}
		}
		// Now process the remaining unprocessed attributes of this node as
		// transient properties
		NamedNodeMap attrs = thisNode.getAttributes();
		if (attrs != null && attrs.getLength() > 0) {
			for (int i = 0; i < attrs.getLength(); i++) {
				Node attNode = attrs.item(i);
				String key = attNode.getLocalName();
				if (key == null)
					key = attNode.getNodeName();
				if (key != null && attNode.getNodeType() == Node.ATTRIBUTE_NODE && !processedAttributes.containsKey(key)) {
					thisObject.updateTransientProperty(key, attNode.getNodeValue());
				}
			}
		}

		NodeList children = thisNode.getChildNodes();
		for (int i = 0; i < children.getLength(); i++) {
			Node n = children.item(i);
			LWAttributeSetContainer childObject = null;
			if (n.getNodeType() != Node.ELEMENT_NODE)
				continue;
			Element child = (Element) n;
			if (child.getNodeName().equals("VirtualCard")) {
				childObject = new VirtualCard();
			} else {
				String typeName = "com.brierley.loyaltyware.clientlib.domainmodel.client." + child.getNodeName();
				Class<?> cl = null;
				try {
					cl = Class.forName(typeName);
				} catch (ClassNotFoundException e) {
					throw new LWClientException("Unable get type for name " + typeName, e, 10002);
				}
				try {
					childObject = (LWAttributeSetContainer) cl.newInstance();
				} catch (InstantiationException e) {
					throw new LWClientException("Unable to instantiate type " + typeName, e, 10002);
				} catch (IllegalAccessException e) {
					throw new LWClientException("Unable to instantiate type " + typeName, e, 10002);
				}
			}
			childObject = deserializeObjectFromXml(child, childObject);
			thisObject.add(childObject);
		}
		return thisObject;
	}

	public Member deserializeMember(String payload) throws LWClientException {
		Document doc = LWUtils.getDocument(payload);
		Element root = doc.getDocumentElement();
		if (root == null) {
			throw new LWClientException("Unexpected response received: " + payload, 1);
		}
		if (!root.getLocalName().equals("AttributeSets")) {
			throw new LWClientException("Unexpected response received: " + payload, 1);
		}
		NodeList nList = root.getElementsByTagName("Member");
		if (nList == null) {
			throw new LWClientException("No member found in the response received: " + payload, 1);
		}
		Element mNode = (Element) nList.item(0);

		// DeserializeMember member
		Member m = new Member();
		m = (Member) deserializeObjectFromXml(mNode, m);

		return m;
	}

	public List<Member> deserializeMembers(String opName, String payload) throws LWClientException {
		List<Member> members = new LinkedList<Member>();
		Document doc = LWUtils.getDocument(payload);
		Element envelope = doc.getDocumentElement();
		if (!envelope.getNodeName().equals(opName + "Return")) {
			throw new LWClientException("Invalid response root received.", 1);
		}
		String isArrayAttribute = envelope.getAttribute("IsArray");
		if (LWUtils.isEmptyString(isArrayAttribute)) {
			throw new LWClientException("Invalid de-serializer called for non array response.", 1);
		}
		Boolean isArray = Boolean.parseBoolean(isArrayAttribute);
		if (!isArray) {
			throw new LWClientException("Invalid de-serializer called for array response.", 1);
		}
		NodeList attSets = envelope.getElementsByTagName("AttributeSets");
		if (attSets.getLength() == 0) {
			// no members returned
		} else {
			// get all members
			XPathFactory xpathFactory = XPathFactory.newInstance();
			XPath xpath = xpathFactory.newXPath();
			try {
				NodeList childNodes = (NodeList) xpath.evaluate("AttributeSets/Member", envelope, XPathConstants.NODESET);
				for (int i = 0; i < childNodes.getLength(); i++) {
					Node e = childNodes.item(i);
					if (e.getNodeType() != Node.ELEMENT_NODE) {
						continue;
					}
					Member m = new Member();
					Element mNode = (Element) e;
					m = (Member) deserializeObjectFromXml(mNode, m);
					members.add(m);
				}
			} catch (XPathExpressionException e1) {
				e1.printStackTrace();
			}
		}
		return members;
	}

	public String deserializeSinglePrimitiveResponse(String opName, String payload) throws LWClientException {
		Document doc = LWUtils.getDocument(payload);
		Element envelope = doc.getDocumentElement();
		if (!envelope.getLocalName().equals(opName + "Return")) {
			throw new LWClientException("Invalid response root received.", 1);
		}
		String isArrayAttribute = envelope.getAttribute("IsArray");
		if (LWUtils.isEmptyString(isArrayAttribute)) {
			throw new LWClientException("Invalid de-serializer called for non array response.", 1);
		}
		Boolean isArray = Boolean.parseBoolean(isArrayAttribute);
		if (isArray) {
			throw new LWClientException("Invalid de-serializer called for array response.", 1);
		}

		Element outParm = (Element) envelope.getFirstChild();
		return outParm.getAttributes().item(0).getNodeValue();
	}

	public Object deserializeDataObject(String opName, String payload) throws LWClientException {
		Document doc = LWUtils.getDocument(payload);
		Element envelope = doc.getDocumentElement();
		if (!envelope.getLocalName().equals(opName + "Return")) {
			throw new LWClientException("Invalid response root received.", 1);
		}
		String isArrayAttribute = envelope.getAttribute("IsArray");
		if (LWUtils.isEmptyString(isArrayAttribute)) {
			throw new LWClientException("Invalid de-serializer called for non array response.", 1);
		}
		Boolean isArray = Boolean.parseBoolean(isArrayAttribute);
		if (isArray) {
			throw new LWClientException("Invalid de-serializer called for array response.", 1);
		}

		Element outParm = (Element) envelope.getFirstChild();

		// create the object instance.
		String typeName = "com.brierley.loyaltyware.clientlib.domainmodel.client." + opName + "Out";
		Class<?> cl = null;
		Object thisObject = null;
		try {
			cl = Class.forName(typeName);
		} catch (ClassNotFoundException e) {
			throw new LWClientException("Unable get type for name " + typeName, e, 10002);
		}
		try {
			thisObject = cl.newInstance();
		} catch (InstantiationException e) {
			throw new LWClientException("Unable to instantiate type " + typeName, e, 10002);
		} catch (IllegalAccessException e) {
			throw new LWClientException("Unable to instantiate type " + typeName, e, 10002);
		}
		if (thisObject == null) {
			throw new LWClientException("Unable to instantiate data object: " + typeName, 1);
		}

		for (Field field : thisObject.getClass().getDeclaredFields()) {
			Object value = null;
			String attValue = outParm.getAttribute(field.getName());
			if (!LWUtils.isEmptyString(attValue)) {
				if (field.getType() == Date.class) {
					value = convertStringToDate(attValue);
				} else if (field.getType() == Long.class) {
					value = Long.parseLong(attValue);
				} else if (field.getType() == Integer.class) {
					value = Integer.parseInt(attValue);
				} else if (field.getType() == Boolean.class) {
					value = Boolean.parseBoolean(attValue);
				} else if (field.getType().isEnum()) {
					value = Enum.valueOf((Class<Enum>) field.getType(), attValue);
				} else {
					value = attValue;
				}
				try {
					field.set(thisObject, value);
				} catch (IllegalArgumentException e) {
					throw new LWClientException("Error setting value of field " + field.getName(), e, 10002);
				} catch (IllegalAccessException e) {
					throw new LWClientException("Error setting value of field " + field.getName(), e, 10002);
				}
			}
		}

		return thisObject;
	}

	@SuppressWarnings("unchecked")
	public List deserializePrimitiveListResponse(String opName, String payload, Class<?> cl) throws LWClientException {
		Document doc = LWUtils.getDocument(payload);
		Element envelope = doc.getDocumentElement();
		if (!envelope.getNodeName().equals(opName + "Return")) {
			throw new LWClientException("Invalid response root received.", 1);
		}
		String isArrayAttribute = envelope.getAttribute("IsArray");
		if (LWUtils.isEmptyString(isArrayAttribute)) {
			throw new LWClientException("Invalid de-serializer called for non array response.", 1);
		}
		Boolean isArray = Boolean.parseBoolean(isArrayAttribute);
		if (!isArray) {
			throw new LWClientException("Invalid de-serializer called for array response.", 1);
		}

		NodeList childNodes = envelope.getChildNodes();
		List list = new ArrayList();
		for (int i = 0; i < childNodes.getLength(); i++) {
			Node e = childNodes.item(i);
			if (e.getNodeType() != Node.ELEMENT_NODE) {
				continue;
			}
			if (cl == String.class) {
				list.add(e.getTextContent());
			} else if (cl == Integer.class) {
				list.add(Integer.parseInt(e.getTextContent()));
			} else if (cl == Long.class) {
				list.add(Long.parseLong(e.getNodeValue()));
			} else if (cl == java.math.BigDecimal.class) {
				list.add(new java.math.BigDecimal(e.getTextContent()));
			} else if (cl == Boolean.class) {
				list.add(Boolean.parseBoolean(e.getTextContent()));
			} else if (cl == Date.class) {
				// list.add(Boolean.parseBoolean(e.getNodeValue()));
			}
		}

		return list;
	}

	public List deserializeDataObjects(String opName, String payload, Class<?> cl) throws LWClientException {
		List list = new LinkedList();
		Document doc = LWUtils.getDocument(payload);
		Element root = doc.getDocumentElement();
		if (!root.getLocalName().equals(opName + "Return")) {
			throw new LWClientException("Invalid response root received.", 1);
		}
		String isArrayAttribute = root.getAttribute("IsArray");
		if (LWUtils.isEmptyString(isArrayAttribute)) {
			throw new LWClientException("Invalid de-serializer called for non array response.", 1);
		}
		Boolean isArray = Boolean.parseBoolean(isArrayAttribute);
		if (isArray) {
			throw new LWClientException("Invalid de-serializer called for array response.", 1);
		}

		Field[] fields = cl.getDeclaredFields();
		NodeList childNodes = root.getChildNodes();
		for (int i = 0; i < childNodes.getLength(); i++) {
			Object thisObject = LWUtils.createInstance(cl);
			for (int fi = 0; fi < fields.length; fi++) {
				Field field = fields[fi];
				Object value = null;
				String attValue = root.getAttribute(field.getName());
				if (!LWUtils.isEmptyString(attValue)) {
					if (field.getType() == Date.class) {
						value = convertStringToDate(attValue);
					} else if (field.getType() == Long.class) {
						value = Long.parseLong(attValue);
					} else if (field.getType() == Integer.class) {
						value = Integer.parseInt(attValue);
					} else if (field.getType() == Boolean.class) {
						value = Boolean.parseBoolean(attValue);
					} else if (field.getType().isEnum()) {
						value = Enum.valueOf((Class<Enum>) field.getType(), attValue);
					} else {
						value = attValue;
					}
					try {
						field.set(thisObject, value);
					} catch (IllegalArgumentException e) {
						throw new LWClientException("Error setting value of field " + field.getName(), e, 10002);
					} catch (IllegalAccessException e) {
						throw new LWClientException("Error setting value of field " + field.getName(), e, 10002);
					}

				}
			}
			list.add(thisObject);
		}

		return list;
	}

	// /////////////////////////////////////////////////////////////////

	private Element EncodeParm(Element node, String name, String type, Boolean isArray, Boolean isRequired) {
		node.setAttribute("Name", name);
		node.setAttribute("Type", type);
		node.setAttribute("IsArray", isArray.toString());
		node.setAttribute("IsRequired", isArray.toString());
		return node;
	}

	private Element AddValue(Document doc, Element element, Object value) {
		Element node = doc.createElement("Value");
		node.appendChild(doc.createTextNode(value.toString()));
		element.appendChild(node);
		return element;
	}

	public String serializeMethodPrimitiveParm(String opName, String parmName, Object parm) {
		Document doc = LWUtils.getDocument();
		Element envelope = doc.createElement("Parm");
		doc.appendChild(envelope);

		if (parm instanceof Boolean) {
			envelope = EncodeParm(envelope, parmName, "Boolean", false, false);
			AddValue(doc, envelope, parm.toString());
		} else if (parm instanceof Long) {
			envelope = EncodeParm(envelope, parmName, "Long", false, false);
			AddValue(doc, envelope, parm.toString());
		} else if (parm instanceof Date) {
			envelope = EncodeParm(envelope, parmName, "Date", false, false);
			Date date = convertStringToDate(parm.toString());
			AddValue(doc, envelope, convertDateToString(date));
		} else if (parm instanceof Integer) {
			envelope = EncodeParm(envelope, parmName, "Integer", false, false);
			AddValue(doc, envelope, parm.toString());
		} else if (parm instanceof java.math.BigDecimal) {
			envelope = EncodeParm(envelope, parmName, "java.math.BigDecimal", false, false);
			AddValue(doc, envelope, parm.toString());
		} else if (parm instanceof String) {
			envelope = EncodeParm(envelope, parmName, "String", false, false);
			AddValue(doc, envelope, parm.toString());
		}
		return LWUtils.getXml(doc);
	}

	public String serializeMethodParm(String opName, String parmName, Object thisObject) throws LWClientException {
		Document doc = LWUtils.getDocument();
		Element envelope = doc.createElement("Parm");
		doc.appendChild(envelope);

		if (thisObject instanceof Member) {
					envelope = EncodeParm(envelope, parmName, "Member", false, false);
					Element memberRoot = doc.createElement("AttributeSets");
					Element memberNode = doc.createElement("Member");
					memberRoot.appendChild(memberNode);
					envelope.appendChild(memberRoot);
					memberNode = serializeObjectToXml(doc, memberNode, (Member) thisObject);
				} else if (thisObject instanceof Member[]) {
					envelope = EncodeParm(envelope, parmName, "Member", true, false);
					Element memberRoot = doc.createElement("AttributeSets");
					Member[] mList = (Member[]) thisObject;
					for (Member m : mList) {
						Element memberNode = doc.createElement("Member");
						memberRoot.appendChild(memberNode);
						memberNode = serializeObjectToXml(doc, memberNode, m);
					}
					envelope.appendChild(memberRoot);
				} else if (thisObject instanceof LWClientDataObject) {
					envelope = EncodeParm(envelope, parmName, "Global", false, false);
					Element globalRoot = doc.createElement("AttributeSets");
					Element globalNode = doc.createElement("Global");
					globalRoot.appendChild(globalNode);
					envelope.appendChild(globalRoot);
					Element attSetNode = doc.createElement(thisObject.getClass().getSimpleName());
					globalNode.appendChild(attSetNode);
					attSetNode = serializeObjectToXml(doc, attSetNode, (LWClientDataObject) thisObject);
				} else if (thisObject instanceof LWClientDataObject[]) {
					envelope = EncodeParm(envelope, parmName, "Global", true, false);
					Element globalRoot = doc.createElement("AttributeSets");
					LWClientDataObject[] mList = (LWClientDataObject[]) thisObject;
					for (LWClientDataObject m : mList) {
						Element globalNode = doc.createElement("Global");
						globalRoot.appendChild(globalNode);
						Element attSetNode = doc.createElement(thisObject.getClass().getSimpleName());
						globalNode.appendChild(attSetNode);
						attSetNode = serializeObjectToXml(doc, attSetNode, m);
					}
					envelope.appendChild(globalRoot);
				}else {
					envelope = EncodeParm(envelope, parmName, thisObject.getClass().getSimpleName(), false, false);
					Class<?> c = thisObject.getClass();
					Field[] pi = c.getDeclaredFields();
					for (Field field : pi) {
						if (!isBrowsable(field)) {
							continue;
						}
						Element parmRoot = null;
						try {
							field.setAccessible(true);
							parmRoot = serializeParm(doc, field, thisObject, 0);
						} catch (IllegalAccessException e) {
							// e.printStackTrace();
						}
						if (parmRoot != null) {
							envelope.appendChild(parmRoot);
						}
					}
		}

		return LWUtils.getXml(doc);
	}

	private Element serializeParm(Document doc, Field field, Object thisObject, int level) throws LWClientException, IllegalAccessException {
//		String msg = "";
//		for (int i = 0; i < level; i++)
//			msg += "   ";
//		msg += "SerializeParm: " + field.getType().getSimpleName() + " " + field.getName();
//		System.out.println(msg);
		if (level > 100) {
			System.err.println("SerializeParm: recursion is too deep: " + level);
			return null;
		}
		Element parmRoot = null;
		Object val = field.get(thisObject);
		if (val != null) {
			parmRoot = doc.createElement("Parm");

			// Member
			if (val instanceof Member) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "Member", false, isRequired(field));
				Element memberRoot = doc.createElement("AttributeSets");
				Element memberNode = doc.createElement("Member");
				memberRoot.appendChild(memberNode);
				parmRoot.appendChild(memberRoot);
				memberNode = serializeObjectToXml(doc, memberNode, (Member) val);
			} else if (val instanceof Member[]) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "Member", true, isRequired(field));
				Element memberRoot = doc.createElement("AttributeSets");
				Element memberNode = doc.createElement("Member");
				memberRoot.appendChild(memberNode);
				parmRoot.appendChild(memberRoot);
				memberNode = serializeObjectToXml(doc, memberNode, (Member) val);
			}

			// Date
			else if (val instanceof Date) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "Date", false, isRequired(field));
				Element e = doc.createElement("Value");
				e.appendChild(doc.createTextNode(convertDateToString((Date) val)));
				parmRoot.appendChild(e);
			} else if (val instanceof Date[]) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "Date", true, isRequired(field));
				Date[] dateval = (Date[]) val;
				for (Date v : dateval) {
					Element e = doc.createElement("Value");
					e.appendChild(doc.createTextNode(convertDateToString((Date) val)));
					parmRoot.appendChild(e);
				}
			}

			// Long
			else if (val instanceof Long) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "Long", false, isRequired(field));
				Element e = doc.createElement("Value");
				e.appendChild(doc.createTextNode(val.toString()));
				parmRoot.appendChild(e);
			} else if (val instanceof Long[]) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "Long", true, isRequired(field));
				Long[] longval = (Long[]) val;
				for (Long v : longval) {
					Element e = doc.createElement("Value");
					if (v != null) {
						e.appendChild(doc.createTextNode(v.toString()));
					}
					parmRoot.appendChild(e);
				}
			}
			// String
			else if (val instanceof String) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "String", false, isRequired(field));
				Element e = doc.createElement("Value");
				e.appendChild(doc.createTextNode((String) val));

				if ( (val == null || "".equals(val.toString())) && isRequired(field) ){
					String errorMessage = field.getName() + " of " + thisObject.getClass().getName() + " is a required property.  Please provide a valid value.";
					throw new IllegalArgumentException(errorMessage);
				}

				parmRoot.appendChild(e);

				int stringLength = getStringLength(field);
				if (val != null && !"".equals(val.toString()) && stringLength > 0 && val.toString().length() > stringLength) {
					String errmsg = "Parm " + field.getName() + " of " + thisObject.getClass().getSimpleName() + " cannot be more than " + stringLength + " characters.";
                    throw new LWClientException(errmsg, 2002);
                }
			} else if (val instanceof String[]) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "String", true, isRequired(field));
				String[] stringval = (String[]) val;
				for (String v : stringval) {
					Element e = doc.createElement("Value");
					if (v != null) {
						e.appendChild(doc.createTextNode((String) v));
					}
					parmRoot.appendChild(e);
				}
			}
			// Integer
			else if (val instanceof Integer) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "Integer", false, isRequired(field));
				Element e = doc.createElement("Value");
				e.appendChild(doc.createTextNode(val.toString()));
				parmRoot.appendChild(e);
			} else if (val instanceof Integer[]) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "Integer", true, isRequired(field));
				Integer[] intval = (Integer[]) val;
				for (Integer v : intval) {
					Element e = doc.createElement("Value");
					if (v != null) {
						e.appendChild(doc.createTextNode(Integer.toString(v)));
					}
					parmRoot.appendChild(e);
				}
			}
			// java.math.BigDecimal
			else if (val instanceof java.math.BigDecimal) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "java.math.BigDecimal", false, isRequired(field));
				Element e = doc.createElement("Value");
				e.appendChild(doc.createTextNode(val.toString()));
				parmRoot.appendChild(e);
			} else if (val instanceof java.math.BigDecimal[]) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "java.math.BigDecimal", true, isRequired(field));
				java.math.BigDecimal[] decimalval = (java.math.BigDecimal[]) val;
				for (java.math.BigDecimal v : decimalval) {
					Element e = doc.createElement("Value");
					if (v != null) {
						e.appendChild(doc.createTextNode(v.toString()));
					}
					parmRoot.appendChild(e);
				}
			}
			// Boolean
			else if (val instanceof Boolean) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "Boolean", false, isRequired(field));
				Element e = doc.createElement("Value");
				e.appendChild(doc.createTextNode(val.toString()));
				parmRoot.appendChild(e);
			} else if (val instanceof Boolean[]) {
				parmRoot = EncodeParm(parmRoot, field.getName(), "Boolean", true, isRequired(field));
				Boolean[] boolval = (Boolean[]) val;
				for (Boolean v : boolval) {
					Element e = doc.createElement("Value");
					if (v != null) {
						e.appendChild(doc.createTextNode(Boolean.toString(v)));
					}
					parmRoot.appendChild(e);
				}
			}
			// Struct
			else { // This must be a struct type
				String structName = field.getName();
				String structTypeName = field.getType().getSimpleName();
				//System.out.println("SerializeParm: struct type: " + structTypeName + " " + structName);
				if (!structTypeName.endsWith("[]")) {
					Object structValue = field.get(thisObject);
					parmRoot = EncodeParm(parmRoot, structName, "Struct", false, isRequired(field));
					Field[] pi = structValue.getClass().getDeclaredFields();
					for (Field subfield : pi) {
						if (!isBrowsable(subfield)) {
							continue;
						}
						subfield.setAccessible(true);
						Element subparmRoot = null;
						try {
							subparmRoot = serializeParm(doc, subfield, structValue, level + 1);
						} catch (IllegalAccessException ex) {
							ex.printStackTrace();
						}
						if (subparmRoot != null) {
							parmRoot.appendChild(subparmRoot);
						}
					}
				} else {
					Object structArrayValue = field.get(thisObject);
					int numItems = Array.getLength(structArrayValue);
					if (numItems > 0) {
						parmRoot = EncodeParm(parmRoot, structName, "Struct", true, isRequired(field));
						for (int itemIndex = 0; itemIndex < numItems; itemIndex++) {
							Element itemElement = doc.createElement("Parm");
							itemElement = EncodeParm(itemElement, structName, "Struct", false, isRequired(field));
							Object itemValue = Array.get(structArrayValue, itemIndex);
							Field[] pi = itemValue.getClass().getDeclaredFields();
							for (Field subfield : pi) {
								if (!isBrowsable(field)) {
									continue;
								}
								subfield.setAccessible(true);
								Element subparmRoot = null;
								try {
									subparmRoot = serializeParm(doc, subfield, itemValue, level + 1);
								} catch (IllegalAccessException ex) {
									ex.printStackTrace();
								}
								if (subparmRoot != null) {
									itemElement.appendChild(subparmRoot);
								}
							}
							parmRoot.appendChild(itemElement);
						}
					} else {
						parmRoot = null;
					}
				}
			}
			// val == null
		} else if (isRequired(field)) {
			String errorMessage = field.getName() + " of " + thisObject.getClass().getName() + " is a required property.  Please provide a valid value.";
			throw new IllegalArgumentException(errorMessage);
		}
		return parmRoot;
	}

	public Object deserializeSingleResponseObject(String opName, String payload) {
		Object[] list = null;
		try {
			list = deserializeResponseObject(opName, payload);
		} catch (Exception e) {
			e.printStackTrace();
		}
		if (list != null && list.length > 0) {
			return list[0];
		} else {
			return null;
		}
	}

	public Object[] deserializeResponseObject(String opName, String payload) throws LWClientException {
		Object[] result = null;
		try {
			String errMsg;
			Document doc = LWUtils.getDocument(payload);
			Element envelop = doc.getDocumentElement();
			if (!(opName + "OutParms").equals(envelop.getNodeName())) {
				errMsg = "Expected '" + opName + "OutParms' as the envelope.  Found " + envelop.getNodeName();
				throw new IllegalArgumentException(errMsg);
			}
			NodeList inParms = envelop.getElementsByTagName("Parm");
			if (inParms == null || inParms.getLength() < 1) {
				errMsg = "Expected 'Parm' as the enclosing type.  Found: " + envelop.getFirstChild().getNodeName();
				throw new IllegalArgumentException(errMsg);
			}

			Element inParm = (Element) inParms.item(0);
			Boolean isArray = Boolean.parseBoolean(getAttributeValue(inParm, "IsArray", "false"));
			String inParmType = getAttributeValue(inParm, "Type", "");
			if ((opName + "Out").equals(inParmType)) {
				if (!isArray) {
					result = new Object[1];
					result[0] = HydrateObject(inParm);
				} else {
					// array of out objects
					NodeList parms = inParm.getChildNodes();
					// .getElementsByTagName("Parm");
					if (parms != null) {
						result = new Object[parms.getLength()];
						int index = 0;
						parms.item(index);
						for (int itemindex = 0; itemindex < parms.getLength(); itemindex++) {
							Element parm = (Element) parms.item(itemindex);
							result[index++] = HydrateObject(parm);
						}
					}
				}
			} else {
				// probably return type was a single element
				if (!isArray) {
					result = new Object[1];
					result[0] = HydrateObject(inParm);
				} else {
					result = (Object[]) HydrateObject(inParm);
				}
			}
		} catch (Exception ex) {
			throw new LWClientException(ex.getLocalizedMessage(), 123);
		}
		return result;

	}

	private List<Element> getElements(Node node, String tagName) {
		List<Element> result = new Vector<Element>();
		NodeList children = node.getChildNodes();
		if (children != null) {
			for (int i = 0; i < children.getLength(); i++) {
				Node child = children.item(i);
				if (child.getNodeType() == Node.ELEMENT_NODE) {
					if (tagName == null || tagName.equals(child.getNodeName())) {
						result.add((Element) child);
					}
				}
			}
		}
		return result;
	}

	private String getAttributeValue(Node node, String attrName, String defaultValue) {
		String result = defaultValue;
		if (node != null) {
			NamedNodeMap attrs = node.getAttributes();
			if (attrs != null) {
				Node attr = attrs.getNamedItem(attrName);
				if (attr != null) {
					result = attr.getNodeValue();
				}
			}
		}
		return result;
	}

	private Object HydrateObject(Element parm) throws InstantiationException, IllegalAccessException, ClassNotFoundException, LWClientException {
		Object result = null;
		String parmName = getAttributeValue(parm, "Name", "");
		boolean isArray = Boolean.parseBoolean(getAttributeValue(parm, "IsArray", "false"));
		// boolean isRequired = Boolean.parseBoolean(getAttributeValue(parm,
		// "IsRequired", "false"));
		String parmType = getAttributeValue(parm, "Type", "");
		if (parmType.endsWith("Out")) {
			if (isArray) {
				List<Object> results = new Vector<Object>();
				String typeName = "com.brierley.loyaltyware.clientlib.domainmodel.client." + parmType;
				List<Element> valueNodes = getElements(parm, "Parm");
				for (Element valueNode : valueNodes) {
					Object thisResult = Class.forName(typeName).newInstance();
					Field[] pi = thisResult.getClass().getDeclaredFields();
					List<Element> subparmNodes = getElements(valueNode, "Parm");
					for (Field field : pi) {
						for (Element subparmNode : subparmNodes) {
							String subparmName = getAttributeValue(subparmNode, "Name", "");
							if (subparmName.equals(field.getName())) {
								Object value = HydrateObject(subparmNode);
								field.set(thisResult, value);
								break;
							}
						}
					}
					results.add(thisResult);
				}
				result = results.toArray();
			} else {
				String typeName = "com.brierley.loyaltyware.clientlib.domainmodel.client." + parmType;
				result = Class.forName(typeName).newInstance();
				Field[] pi = result.getClass().getDeclaredFields();
				List<Element> subparmNodes = getElements(parm, "Parm");
				for (Field field : pi) {
					field.setAccessible(true);
					String fieldName = field.getName();
					for (Element subparmNode : subparmNodes) {
						String subparmName = getAttributeValue(subparmNode, "Name", "");
						if (subparmName.equals(fieldName)) {
							Object value = HydrateObject(subparmNode);
							field.set(result, value);
							break;
						}
					}
				}
			}
		} else {
			if ("Integer".equals(parmType)) {
				if (isArray) {
					List<Element> valueNodes = getElements(parm, "Value");
					Integer[] vals = new Integer[valueNodes.size()];
					int index = 0;
					for (Element valueNode : valueNodes) {
						String valueNodeVal = valueNode.getTextContent().trim();
						vals[index++] = Integer.parseInt(valueNodeVal);
					}
					result = vals;
				} else {
					String nodeval = parm.getTextContent().trim();
					Integer val = Integer.parseInt(nodeval);
					result = val;
				}
			} else if ("Long".equals(parmType)) {
				if (isArray) {
					List<Element> valueNodes = getElements(parm, "Value");
					Long[] vals = new Long[valueNodes.size()];
					int index = 0;
					for (Element valueNode : valueNodes) {
						String valueNodeVal = valueNode.getTextContent().trim();
						vals[index++] = Long.parseLong(valueNodeVal);
					}
					result = vals;
				} else {
					String nodeval = parm.getTextContent().trim();
					Long val = Long.parseLong(nodeval);
					result = val;
				}
			} else if ("Decimal".equals(parmType)) {
				if (isArray) {
					List<Element> valueNodes = getElements(parm, "Value");
					java.math.BigDecimal[] vals = new java.math.BigDecimal[valueNodes.size()];
					int index = 0;
					for (Element valueNode : valueNodes) {
						String valueNodeVal = valueNode.getTextContent().trim();
						vals[index++] = new java.math.BigDecimal(valueNodeVal);
					}
					result = vals;
				} else {
					String nodeval = parm.getTextContent().trim();
					java.math.BigDecimal val = new java.math.BigDecimal(nodeval);
					result = val;
				}
			} else if ("Boolean".equals(parmType)) {
				if (isArray) {
					List<Element> valueNodes = getElements(parm, "Value");
					Boolean[] vals = new Boolean[valueNodes.size()];
					int index = 0;
					for (Element valueNode : valueNodes) {
						String valueNodeVal = valueNode.getTextContent().trim();
						vals[index++] = Boolean.parseBoolean(valueNodeVal);
					}
					result = vals;
				} else {
					String nodeval = parm.getTextContent().trim();
					Boolean val = Boolean.parseBoolean(nodeval);
					result = val;
				}
			} else if ("String".equals(parmType)) {
				if (isArray) {
					List<Element> valueNodes = getElements(parm, "Value");
					String[] vals = new String[valueNodes.size()];
					int index = 0;
					for (Element valueNode : valueNodes) {
						vals[index++] = valueNode.getTextContent().trim();
					}
					result = vals;
				} else {
					String val = parm.getTextContent().trim();
					result = val;
				}
			} else if ("Date".equals(parmType)) {
				if (isArray) {
					List<Element> valueNodes = getElements(parm, "Value");
					Date[] vals = new Date[valueNodes.size()];
					int index = 0;
					for (Element valueNode : valueNodes) {
						String valueNodeVal = valueNode.getTextContent().trim();
						Date date = convertStringToDate(valueNodeVal);
						vals[index++] = date;
					}
					result = vals;
				} else {
					String nodeval = parm.getTextContent().trim();
					Date date = convertStringToDate(nodeval);
					result = date;
				}
			} else if ("Struct".equals(parmType)) {
				if (isArray) {
					List<Object> results = new Vector<Object>();
					String typeName = "com.brierley.loyaltyware.clientlib.domainmodel.client." + parmName + "Struct";
					List<Element> valueNodes = getElements(parm, "Parm");
					for (Element valueNode : valueNodes) {
						Object thisResult = Class.forName(typeName).newInstance();
						Field[] pi = thisResult.getClass().getDeclaredFields();
						List<Element> parmNodes = getElements(valueNode, "Parm");
						for (Field field : pi) {
							field.setAccessible(true);
							String fieldName = field.getName();
							for (Element parmNode : parmNodes) {
								String parmNodeName = getAttributeValue(parmNode, "Name", "");
								if (parmNodeName.equals(fieldName)) {
									Object value = HydrateObject(parmNode);
									field.set(thisResult, value);
									break;
								}
							}
						}
						results.add(thisResult);
					}
					result = Array.newInstance(Class.forName(typeName), results.size());
					for (int index = 0; index < results.size(); index++) {
						Array.set(result, index, results.get(index));
					}
				} else {
					String typeName = "com.brierley.loyaltyware.clientlib.domainmodel.client." + parmName + "Struct";
					result = Class.forName(typeName).newInstance();
					Field[] pi = result.getClass().getDeclaredFields();
					List<Element> parmNodes = getElements(parm, "Parm");
					for (Field field : pi) {
						field.setAccessible(true);
						String subparmName = field.getName();
						//System.out.println("HydrateObject: " + subparmName);
						for (Element subparmNode : parmNodes) {
							String subparmNodeName = getAttributeValue(subparmNode, "Name", "");
							if (subparmNodeName.equals(subparmName)) {
								Object value = HydrateObject(subparmNode);
								if (value instanceof Object[]) {
									Object[] values = (Object[]) value;
									String arrayTypeName = "com.brierley.loyaltyware.clientlib.domainmodel.client." + subparmName + "Struct";
									Object arrayResult = Class.forName(arrayTypeName).newInstance();
									Object a = Array.newInstance(arrayResult.getClass(), values.length);
									for (int i = 0; i < values.length; i++) {
										Array.set(a, i, values[i]);
									}
									field.set(result, a);
								} else {
									field.set(result, value);
								}
								break;
							}
						}
					}
				}
			} else if ("Member".equals(parmType)) {
				List<Element> attSets = getElements(parm, "AttributeSets");
				if (attSets == null) {
					throw new LWClientException("Unexpected response received.  No content under AttributeSets received.", 2000);
				}
				if (!isArray) {
					List<Element> mNodes = getElements(attSets.get(0), "Member");
					if (mNodes == null || mNodes.size() < 1) {
						throw new LWClientException("No member found in the response received.", 2001);
					}
					Member m = new Member();
					m = (Member) deserializeObjectFromXml(mNodes.get(0), m);
					result = m;
				} else {
					List<Object> results = new Vector<Object>();
					List<Element> mNodes = getElements(attSets.get(0), "Member");
					for (Element mNode : mNodes) {
						Member m = new Member();
						m = (Member) deserializeObjectFromXml(mNode, m);
						results.add(m);
					}
					result = Array.newInstance(Member.class, results.size());
					for (int index = 0; index < results.size(); index++) {
						Array.set(result, index, results.get(index));
					}
				}
			}
		}
		return result;
	}
}
