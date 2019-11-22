package com.brierley.loyaltyware.clientlib;

public class LWClientException extends Exception {
	private static final long serialVersionUID = 1L;
	private int errorCode;

	public int getErrorCode() {
		return errorCode;
	}

	public void setErrorCode(int errorCode) {
		this.errorCode = errorCode;
	}

    private double elapsedTime;

	public double getElapsedTime() {
		return elapsedTime;
	}

	public void setElapsedTime(double elapsedTime) {
		this.elapsedTime = elapsedTime;
	}

	public LWClientException(int errorCode) {
		setErrorCode(errorCode);
	}

	public LWClientException(String arg0,int errorCode) {
		super(arg0);
		setErrorCode(errorCode);
	}

	public LWClientException(Throwable arg0,int errorCode) {
		super(arg0);
		setErrorCode(errorCode);
	}

	public LWClientException(String arg0, Throwable arg1,int errorCode) {
		super(arg0, arg1);
		setErrorCode(errorCode);
	}

	public LWClientException(String arg0, int errorCode, double elapsedTime) {
		super(arg0);
		setErrorCode(errorCode);
		setElapsedTime(elapsedTime);
	}

}
