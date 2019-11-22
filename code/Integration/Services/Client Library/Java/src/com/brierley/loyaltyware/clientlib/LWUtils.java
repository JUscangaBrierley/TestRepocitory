package com.brierley.loyaltyware.clientlib;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.StringWriter;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerConfigurationException;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.xml.sax.SAXException;

public class LWUtils {

	public static String capitalize(String str){
		StringBuilder sb = new StringBuilder(str);
		int i = 0;
		do{
			sb.replace(i,i+1, sb.substring(i,i + 1).toUpperCase());
		} while (i > 0 && i < sb.length());
		return sb.toString();
	}

	public static Document getDocument(){
		DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
    	DocumentBuilder parser = null;
		try {
			parser = factory.newDocumentBuilder();
		} catch (ParserConfigurationException e) {
			e.printStackTrace();
			return null;
		}
    	Document doc = parser.newDocument();
    	return doc;
	}

	public static Document getDocument(String xmlString){
		DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
    	DocumentBuilder parser = null;
		try {
			parser = factory.newDocumentBuilder();
		} catch (ParserConfigurationException e) {
			e.printStackTrace();
			return null;
		}
		ByteArrayInputStream in = new ByteArrayInputStream(xmlString.getBytes());
    	Document doc = null;
		try {
			doc = parser.parse(in);
		} catch (SAXException e) {
			e.printStackTrace();
			return null;
		} catch (IOException e) {
			e.printStackTrace();
			return null;
		}
    	return doc;
	}

	public static String getXml(Document doc){

		//set up a transformer
        TransformerFactory transfac = TransformerFactory.newInstance();
        Transformer trans = null;
		try {
			trans = transfac.newTransformer();
		} catch (TransformerConfigurationException e) {
			e.printStackTrace();
			return null;
		}
        trans.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
        trans.setOutputProperty(OutputKeys.INDENT, "yes");


		StringWriter sw = new StringWriter();
        StreamResult  result = new StreamResult (sw);
        DOMSource source = new DOMSource(doc);
        try {
			trans.transform(source, result);
		} catch (TransformerException e) {
			e.printStackTrace();
			return null;
		}
        String xmlString = sw.toString();
        return xmlString;
	}

	public static String getXml(Element node){

		//set up a transformer
        TransformerFactory transfac = TransformerFactory.newInstance();
        Transformer trans = null;
		try {
			trans = transfac.newTransformer();
		} catch (TransformerConfigurationException e) {
			e.printStackTrace();
			return null;
		}
        trans.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
        trans.setOutputProperty(OutputKeys.INDENT, "yes");


		StringWriter sw = new StringWriter();
        StreamResult  result = new StreamResult (sw);
        DOMSource source = new DOMSource(node);
        try {
			trans.transform(source, result);
		} catch (TransformerException e) {
			e.printStackTrace();
			return null;
		}
        String xmlString = sw.toString();
        return xmlString;
	}

	public static Boolean isEmptyString(String s){
		return s != null && s.trim().length() > 0 ? false : true;
	}

	@SuppressWarnings("unchecked")
	public static Object createInstance(String typeName) throws LWClientException{
        Class cl = null;
        try {
			cl = Class.forName(typeName);
		} catch (ClassNotFoundException e) {
			e.printStackTrace();
			return null;
		}
        return createInstance(cl);
	}

	@SuppressWarnings("unchecked")
	public static Object createInstance(Class cl) throws LWClientException{
        Object thisObject = null;
        try {
        	thisObject = cl.newInstance();
		} catch (InstantiationException e) {
			e.printStackTrace();
			return null;
		} catch (IllegalAccessException e) {
			e.printStackTrace();
			return null;
		}
        if (thisObject == null)
        {
            throw new LWClientException("Unable to instantiate data object: " + cl.getName(), 1);
        }
        return thisObject;
	}
}
