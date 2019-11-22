create or replace package cio_mail is

   /**
   * E-mail related fuctionality.  The primary method is cio_mail.send, however several simplified wrappers
   * are provided as well.
   *
   * %version CleverIdeasForOracle-september2006a
   *
   * %author © Copyright 2002-2006 Michael John O'Neill of Clever Idea Consulting
   * %author Please visit http://cleveridea.net for more information
   * %author Licensed under GNU GPL v2 (http://www.gnu.org/licenses/licenses.html#TOCGPL)
   */


   /**
   * Oracle Reserved RDBMS Exceptions.
   *
   * This convenience package puts a "name" to a variety of relevant Oracle exception codes.
   * Use the constants below with {%link cio_x.html#throw cio.throw}(p_exception=>[constant]) calls and the named
   * exceptions in exception block handlers.
   */

   this_package CONSTANT VARCHAR2(33) := 'cio_mail.';

   "cannot acquire lock"	    CONSTANT VARCHAR2(65) := this_package || 'cannot_acquire_lock';
   "comparing intervals and dates"  CONSTANT VARCHAR2(65) := this_package || 'comparing_intervals_and_dates';
   "date format not recognized"     CONSTANT VARCHAR2(65) := this_package || 'date_format_not_recognized';
   "date invalid for calendar"	    CONSTANT VARCHAR2(65) := this_package || 'date_invalid_for_calendar';
   "deadlock detected"		    CONSTANT VARCHAR2(65) := this_package || 'deadlock_detected';
   "expected xml got no content"    CONSTANT VARCHAR2(65) := this_package || 'expected_xml_got_no_content';
   "format code appears twice"	    CONSTANT VARCHAR2(65) := this_package || 'format_code_appears_twice';
   "identifier is too long"	    CONSTANT VARCHAR2(65) := this_package || 'identifier_is_too_long';
   "insufficient privileges"	    CONSTANT VARCHAR2(65) := this_package || 'insufficient_privileges';
   "not a valid era"		    CONSTANT VARCHAR2(65) := this_package || 'not_a_valid_era';
   "missing expresion"		    CONSTANT VARCHAR2(65) := this_package || 'missing_expresion';
   "nls error detected" 	    CONSTANT VARCHAR2(65) := this_package || 'nls_error_detected';
   "not a valid hh24 hour"	    CONSTANT VARCHAR2(65) := this_package || 'not_a_valid_hh24_hour';
   "not a valid interval"	    CONSTANT VARCHAR2(65) := this_package || 'not_a_valid_interval';
   "not a valid julian date"	    CONSTANT VARCHAR2(65) := this_package || 'not_a_valid_julian_date';
   "not a valid month"		    CONSTANT VARCHAR2(65) := this_package || 'not_a_valid_month';
   "not a valid time zone hour"     CONSTANT VARCHAR2(65) := this_package || 'not_a_valid_time_zone_hour';
   "not a valid timezone region"    CONSTANT VARCHAR2(65) := this_package || 'not_a_valid_timezone_region';
   "not a valid timezone region id" CONSTANT VARCHAR2(65) := this_package || 'not_a_valid_timezone_region_id';
   "not a valid year"		    CONSTANT VARCHAR2(65) := this_package || 'not_a_valid_year';
   "not valid fractional seconds"   CONSTANT VARCHAR2(65) := this_package || 'not_valid_fractional_seconds';
   "not valid seconds"		    CONSTANT VARCHAR2(65) := this_package || 'not_valid_seconds';
   "not valid time zone minutes"    CONSTANT VARCHAR2(65) := this_package || 'not_valid_time_zone_minutes';
   "numeric val exceeded precision" CONSTANT VARCHAR2(65) := this_package || 'numeric_val_exceeded_precision';
   "plsql compilation error"	    CONSTANT VARCHAR2(65) := this_package || 'plsql_compilation_error';
   "oracle in restricted session"   CONSTANT VARCHAR2(65) := this_package || 'oracle_in_restricted_session';
   "oracle initial or shutdown"     CONSTANT VARCHAR2(65) := this_package || 'oracle_initial_or_shutdown';
   "source too large"		    CONSTANT VARCHAR2(65) := this_package || 'source_too_large';
   "sql command not properly ended" CONSTANT VARCHAR2(65) := this_package || 'sql_command_not_properly_ended';
   "unexpected non alpha found"     CONSTANT VARCHAR2(65) := this_package || 'unexpected_non_alpha_found';
   "unexpected non numeric found"   CONSTANT VARCHAR2(65) := this_package || 'unexpected_non_numeric_found';
   "xml parsing failed" 	    CONSTANT VARCHAR2(65) := this_package || 'xml_parsing_failed';
   "year invalid for calendar"	    CONSTANT VARCHAR2(65) := this_package || 'year_invalid_for_calendar';
   "year is too long ago"	    CONSTANT VARCHAR2(65) := this_package || 'year_is_too_long_ago';

   cannot_acquire_lock EXCEPTION;
   comparing_intervals_and_dates EXCEPTION;
   date_format_not_recognized EXCEPTION;
   date_invalid_for_calendar EXCEPTION;
   deadlock_detected EXCEPTION;
   expected_xml_got_no_content EXCEPTION;
   format_code_appears_twice EXCEPTION;
   identifier_is_too_long EXCEPTION;
   insufficient_privileges EXCEPTION;
   not_a_valid_era EXCEPTION;
   missing_expresion EXCEPTION;
   nls_error_detected EXCEPTION;
   not_a_valid_hh24_hour EXCEPTION;
   not_a_valid_interval EXCEPTION;
   not_a_valid_julian_date EXCEPTION;
   not_a_valid_month EXCEPTION;
   not_a_valid_time_zone_hour EXCEPTION;
   not_a_valid_timezone_region EXCEPTION;
   not_a_valid_timezone_region_id EXCEPTION;
   not_a_valid_year EXCEPTION;
   not_valid_fractional_seconds EXCEPTION;
   not_valid_seconds EXCEPTION;
   not_valid_time_zone_minutes EXCEPTION;
   numeric_val_exceeded_precision EXCEPTION;
   plsql_compilation_error EXCEPTION;
   oracle_in_restricted_session EXCEPTION;
   oracle_initial_or_shutdown EXCEPTION;
   source_too_large EXCEPTION;
   sql_command_not_properly_ended EXCEPTION;
   unexpected_non_alpha_found EXCEPTION;
   unexpected_non_numeric_found EXCEPTION;
   xml_parsing_failed EXCEPTION;
   year_invalid_for_calendar EXCEPTION;
   year_is_too_long_ago EXCEPTION;

   PRAGMA EXCEPTION_INIT(deadlock_detected, -60);
   PRAGMA EXCEPTION_INIT(cannot_acquire_lock, -69);
   PRAGMA EXCEPTION_INIT(sql_command_not_properly_ended, -933);
   PRAGMA EXCEPTION_INIT(missing_expresion, -936);
   PRAGMA EXCEPTION_INIT(identifier_is_too_long, -972);
   PRAGMA EXCEPTION_INIT(insufficient_privileges, -1031);
   PRAGMA EXCEPTION_INIT(oracle_initial_or_shutdown, -1033);
   PRAGMA EXCEPTION_INIT(numeric_val_exceeded_precision, -1438);
   PRAGMA EXCEPTION_INIT(format_code_appears_twice, -1810);
   PRAGMA EXCEPTION_INIT(date_format_not_recognized, -1821);
   PRAGMA EXCEPTION_INIT(not_a_valid_year, -1841);
   PRAGMA EXCEPTION_INIT(not_a_valid_month, -1843);
   PRAGMA EXCEPTION_INIT(not_a_valid_julian_date, -1854);
   PRAGMA EXCEPTION_INIT(unexpected_non_numeric_found, -1858);
   PRAGMA EXCEPTION_INIT(unexpected_non_alpha_found, -1859);
   PRAGMA EXCEPTION_INIT(year_invalid_for_calendar, -1863);
   PRAGMA EXCEPTION_INIT(date_invalid_for_calendar, -1864);
   PRAGMA EXCEPTION_INIT(not_a_valid_era, -1865);
   PRAGMA EXCEPTION_INIT(not_a_valid_interval, -1867);
   PRAGMA EXCEPTION_INIT(comparing_intervals_and_dates, -1870);
   PRAGMA EXCEPTION_INIT(not_valid_seconds, -1871);
   PRAGMA EXCEPTION_INIT(not_a_valid_time_zone_hour, -1874);
   PRAGMA EXCEPTION_INIT(not_valid_time_zone_minutes, -1875);
   PRAGMA EXCEPTION_INIT(year_is_too_long_ago, -1876);
   PRAGMA EXCEPTION_INIT(not_a_valid_hh24_hour, -1879);
   PRAGMA EXCEPTION_INIT(not_valid_fractional_seconds, -1880);
   PRAGMA EXCEPTION_INIT(not_a_valid_timezone_region_id, -1881);
   PRAGMA EXCEPTION_INIT(not_a_valid_timezone_region, -1882);
   PRAGMA EXCEPTION_INIT(nls_error_detected, -1890);
   PRAGMA EXCEPTION_INIT(plsql_compilation_error, -6550);
   PRAGMA EXCEPTION_INIT(expected_xml_got_no_content, -19032);
   PRAGMA EXCEPTION_INIT(source_too_large, -29330);
   PRAGMA EXCEPTION_INIT(xml_parsing_failed, -31011);
   /**
   * Core exceptions defined for CleverIdeasForOracle (cio) packages.  This package extends the
   * functionality of the {%link cio_x cio_x} package.	Create your own package rather than extending
   * this package or you risk losing those modifications when updgrading your CleverIdeasForOracle objects
   * in the future.
   *
   * Use the constants below with {%link cio_x.html#throw cio_x.throw}(p_exception=>[constant]) parameter.
   * Identifiers for these collections should be named similar to corresponding exception.  I use
   * case-sensitive identifiers in double-quotes without the spaces, but that is not required.
   *
   * Define your error stack message (with replaceable tokens!) in the body initialization.  Include that
   * message as a comment in the specification as a courtesy for the consumers of your exceptions.
   */


   /**
   * This function and exact signature is required by cio_x.throw() to return stylized error stack messages.
   * See package body initialization.
   */
   FUNCTION error_stack_message(p_exception IN VARCHAR2) RETURN VARCHAR2;


   /**
   * (used internally).  Shorthand for constant declarations.
   */

   "collection is not one based"    CONSTANT VARCHAR2(65) := this_package || 'collection_is_not_one_based';
   "exception does not exist"	    CONSTANT VARCHAR2(65) := this_package || 'exception_does_not_exist';
   "exception outside user range"   CONSTANT VARCHAR2(65) := this_package || 'exception_outside_user_range';
   "fatal html syntax error"	    CONSTANT VARCHAR2(65) := this_package || 'fatal_html_syntax_error';
   "generic exception"		    CONSTANT VARCHAR2(65) := this_package || 'generic_exception';
   "invalid user"		    CONSTANT VARCHAR2(65) := this_package || 'invalid_user';
   "lookup table is corrupt"	    CONSTANT VARCHAR2(65) := this_package || 'lookup_table_is_corrupt';
   "mandatory decision tree failed" CONSTANT VARCHAR2(65) := this_package || 'mandatory_decision_tree_failed';
   "package state undefined"	    CONSTANT VARCHAR2(65) := this_package || 'package_state_undefined';
   "parameter cannot be null"	    CONSTANT VARCHAR2(65) := this_package || 'parameter_cannot_be_null';
   "parameter did not conform"	    CONSTANT VARCHAR2(65) := this_package || 'parameter_did_not_conform';
   "security violation" 	    CONSTANT VARCHAR2(65) := this_package || 'security_violation';
   "sparse collection not allowed"  CONSTANT VARCHAR2(65) := this_package || 'sparse_collection_not_allowed';
   "string too large"		    CONSTANT VARCHAR2(65) := this_package || 'string_too_large';


   /**
   * ORA-20004: an associative array/nested table %1 must be one-based in this context %2
   */
   collection_is_not_one_based EXCEPTION;


   /**
   * ORA-20010: the exception %1 does not exist in the user define-able range
   */
   exception_outside_user_range EXCEPTION;


   /**
   * ORA-20012:the exception %1 does not exist
   */
   exception_does_not_exist EXCEPTION;


   /**
   * ORA-20007: the PL/SQL Gateway detected a fatal HTML syntax error %1
   */
   fatal_html_syntax_error EXCEPTION;


   /**
   * ORA-20000: should not occur in production software %1
   */
   generic_exception EXCEPTION;


   /**
   * ORA-20009: the user %1 is invalid in this context %2
   */
   invalid_user EXCEPTION;


   /**
   * ORA-20005: a lookup table %1 has data integrity violations in this context %2
   */
   lookup_table_is_corrupt EXCEPTION;


   /**
   * ORA-20011: the decision tree failed to make a mandatory choice for %1
   */
   mandatory_decision_tree_failed EXCEPTION;


   /**
   * ORA-20008: the package variable/constant %1 doesn''t exist or is unexpectedly NULL
   */
   package_state_undefined EXCEPTION;


   /**
   * ORA-20002: an unhandled null parameter %1 was passed in this context %2
   */
   parameter_cannot_be_null EXCEPTION;


   /**
   * ORA-20006: the parameter value %1 did not conform to the expected format %2 in this context %3
   */
   parameter_did_not_conform EXCEPTION;


   /**
   * ORA-20013: there was security violation of %1 in this context %2
   */
   security_violation EXCEPTION;


   /**
   * ORA-20003: a sparse associative array or nested table %1 is not allowed in this context %2
   */
   sparse_collection_not_allowed EXCEPTION;


   /**
   * ORA-20001: string %1 length exceeded limit in this context %2'
   */
   string_too_large EXCEPTION;


   PRAGMA EXCEPTION_INIT(security_violation, -20013);
   PRAGMA EXCEPTION_INIT(exception_does_not_exist, -20012);
   PRAGMA EXCEPTION_INIT(mandatory_decision_tree_failed, -20011);
   PRAGMA EXCEPTION_INIT(exception_outside_user_range, -20010);
   PRAGMA EXCEPTION_INIT(invalid_user, -20009);
   PRAGMA EXCEPTION_INIT(package_state_undefined, -20008);
   PRAGMA EXCEPTION_INIT(fatal_html_syntax_error, -20007);
   PRAGMA EXCEPTION_INIT(parameter_did_not_conform, -20006);
   PRAGMA EXCEPTION_INIT(lookup_table_is_corrupt, -20005);
   PRAGMA EXCEPTION_INIT(collection_is_not_one_based, -20004);
   PRAGMA EXCEPTION_INIT(sparse_collection_not_allowed, -20003);
   PRAGMA EXCEPTION_INIT(parameter_cannot_be_null, -20002);
   PRAGMA EXCEPTION_INIT(string_too_large, -20001);
   PRAGMA EXCEPTION_INIT(generic_exception, -20000);
   /**
   * Sophisticated exception throwing package.
   *
   * Encourages the replacement of sporadic, conflicting, scattered and cryptic declared exceptions
   * across PL/SQL sources.  Entirely replaces all hardcoding of exception codes and the use of
   * raise_application_error() calls.  If logging_enable() is true, throw() calls are logged to
   * {%link cio_x_log.html cio_x_log} table.  See {%link cio_x_oracle.html cio_x_oracle} and
   * {%link cio_x_core.html cio_x_core} packages for examples on extending the functionality of this
   * package in your environment.
   *
   * %author ? Copyright 2002-2006 Michael John O'Neill of Clever Idea Consulting
   * %author Please visit http://cleveridea.net for more information
   * %author Licensed under GNU GPL v2 (http://www.gnu.org/licenses/licenses.html#TOCGPL)
   *
   * %feature 100% elimination of raise_application_error() calls
   * %feature Your exceptions can now return stylized error messages with dynamic token content, just like Oracle exceptions
   * %feature Optional autonomously transaction logging of your exception events which is dbms_application_info aware.
   */


   /**
   * Datatype used in several of this package's methods.
   */
   TYPE error_stack_token_table_type IS TABLE OF VARCHAR2(4000) INDEX BY VARCHAR2(256);


   /**
   * An empty PL/SQL Table variable used as default in several methods in this package, and is not intended as a
   * value placeholder in anyway.
   */
   g_empty_tokens error_stack_token_table_type;


   /**
   * The SQLCODE of any named exception.
   *
   * %param p_exception The string literal of any named exception in any package.
   *
   * %usage <code>dbms_output.put_line( to_char(cio_x.code('invalid_number')) ); -- outputs -1722</code>
   */
   FUNCTION CODE(p_exception IN VARCHAR2) RETURN INTEGER;



   /**
   * Enables default logging behavior of subsequent of throw() calls.  Even if this is not called,
   * each throw() can individually pass the argument p_logging=>true to override logging_status.
   */
   PROCEDURE logging_enable;


   /**
   * Logical reverse of logging_enable.
   */
   PROCEDURE logging_disable;


   /**
   * Returns status, used as default behavior for throw() calls.  use logging_enable and logging_disable to
   * change this value.
   */
   FUNCTION logging_status RETURN BOOLEAN;


   /**
   * Deletes all persistent logging data from {%link cio_x_log.html cio_x_log} table.
   */
   PROCEDURE purge_logging;


   /**
   * Retreives most recent logging event
   *
   * %return record type of {%link cio_x_log.html cio_x_log} table
   */
   FUNCTION get_last_logging RETURN cio_x_log%ROWTYPE;


   /**
   * Use instead of RAISE <exception> in your <b>exception blocks</b>.
   *
   * %param p_exception Any named exception in any package.  [schema.][package.]exception format is expected.
   *	    It is recommended that you establish string constants in your custom exception packages to avoid
   *	    hardcoding strings into your throw() calls in your applications.
   *	    See core_x_core package for example on how to do this.
   * %param p_tokens The developer generated token stack to be used in error stack message generation for
   *	    user-defined exceptions in cio_x-friendly packages (see: {%link cio_x_oracle.html cio_x_oracle} and
   *	    {%link cio_x_core.html cio_x_core} for examples).
   *	    When your throw() call has no relevant tokens to supply, use g_empty_tokens as a null-equivalent.
   * %param p_logging Pass true to enable autonomously transacted logging to {%link cio_x_log.html cio_x_log}
   *	    table (default is the current value of logging_status).  Only pass a value for this parameter if
   *	    you intend to override the current logging_status value.
   *
   * %raises cio_x_core.exception_does_not_exist if p_exception is not an exception defined in your database.
   *
   * %usage Should be called in place of any raise or raise_application_error call in your exception block.
   *	    Do not use throw() in the execution block; continue to use RAISE as you normally would.
   * <code>
   * declare
   *	v_tokens cio_x.error_stack_token_table_type;
   * begin
   *	v_tokens(1) := 'test';
   *
   *	if condition_one
   *	then
   *	   cio_x.throw( 'cio_x_core.generic_exception', v_tokens );
   *	   cio_x.throw( cio_x_core."generic exception"', v_tokens ); -- even better, uses a constant
   *	   cio_x.throw( cio_x_core.generic_exception, v_tokens ); -- not allowable, string literal required
   *	elsif condition_two
   *	then
   *	   cio_x.throw( 'no_data_found' ); -- works with builtin exceptions and ones you already have, seemlessly
   *	end if;
   * exception
   *	when cio_x_core.generic_exception then
   *	   do_something;
   *	when no_data_found then
   *	   do_something;
   *	   cio_x.throw('YourPackage.YourException');
   *	when others then
   *	   raise; -- doesn't replace pure re-raise <i>"raise;"</i> calls in execution block
   * end;
   * </code>
   */
   PROCEDURE throw
   (
      p_exception IN VARCHAR2
     ,p_tokens	  IN error_stack_token_table_type
     ,p_logging   IN BOOLEAN DEFAULT logging_status
   );


   /**
   * Returns value set by dbms_application_info.set_client_info.  Used by logging mechanism.
   */
   FUNCTION get_application_client_info RETURN VARCHAR2;


   /**
   * Returns value set by dbms_application_info.set_module.  Used by logging mechanism.
   */
   FUNCTION get_application_module RETURN VARCHAR2;


   /**
   * Returns value set by dbms_application_info.set_action.  Used by logging mechanism.
   */
   FUNCTION get_application_action RETURN VARCHAR2;



/**
    * Predefined PL/SQL Exceptions/Pragmas.
    *
    * When extending cio_x, you shouldn't define pragmas that duplicate the following buitlins exceptions
    *
    *	 ORA-00000 (normal execution)
    *	 ORA-00001 DUP_VAL_ON_INDEX
    *	 ORA-00051 TIMEOUT_ON_RESOURCE
    *	 ORA-00100 NO_DATA_FOUND (ANSI)
    *	 ORA-01001 INVALID_CURSOR
    *	 ORA-01403 NO_DATA_FOUND (Oracle)
    *	 ORA-01410 SYS_INVALID_ROWID
    *	 ORA-01422 TOO_MANY_ROWS
    *	 ORA-01476 ZERO_DIVIDE
    *	 ORA-01722 INVALID_NUMBER
    *	 ORA-06504 ROWTYPE_MISMATCH
    *	 ORA-06511 CURSOR_ALREADY_OPEN
    *	 ORA-06530 ACCESS_INTO_NULL
    *	 ORA-06531 COLLECTION_IS_NULL
    *	 ORA-06532 SUBSCRIPT_OUTSIDE_LIMIT
    *	 ORA-06533 SUBSCRIPT_BEYOND_COUNT
    *	 ORA-06592 CASE_NOT_FOUND
    *	 ORA-30625 SELF_IS_NULL
    *	 ORA-00051 TIMEOUT_ON_RESOURCE
    *	 ORA-06502 VALUE_ERROR
    */


   /**
   * A variety of utilities useful for general usage.
   *
   * %version CleverIdeasForOracle-august2006c
   *
   * %author ? Copyright 2002-2006 Michael John O'Neill of Clever Idea Consulting
   * %author Please visit http://cleveridea.net for more information
   * %author Licensed under GNU GPL v2 (http://www.gnu.org/licenses/licenses.html#TOCGPL)
   */


   /**
   * Converts character-separated list into a nested table.
   *
   * %param p_list The list (16,000 elements maximum)
   * %param p_separator The single character separator (',' is default)
   * %return The nested table.
   */
   FUNCTION list_to_table
   (
      p_list	  IN VARCHAR2
     ,p_separator IN VARCHAR2 DEFAULT ','
   ) RETURN cio_plsql_string_array;


   /**
   * Converts a nested table into a character separated list
   *
   * %param p_table The table to convert (sum of elements and separators cannot exceed 32k bytes)
   * %return The list.
   */
   FUNCTION table_to_list
   (
      p_table	  IN cio_plsql_string_array
     ,p_separator IN VARCHAR2 DEFAULT ','
   ) RETURN VARCHAR2;

   FUNCTION clob_to_blob (c IN cLOB)  RETURN BLOB;
   /**
   * Interesting functionality for working with the clob datatype.
   *
   * %author ? Copyright 2002-2006 Michael John O'Neill of Clever Idea Consulting
   * %author Please visit http://cleveridea.net for more information
   * %author Licensed under GNU GPL v2 (http://www.gnu.org/licenses/licenses.html#TOCGPL)
   */


   /**
   * Submits a Clob to the PL/SQL engine for execution.  Great for use in applications that create,
   * persist in tables and execute PL/SQL on demand.
   *
   * %param p_clob Must be a single block of PL/SQL, can be trailed with or without slash "/"
   */
   PROCEDURE execute_plsql(p_clob IN CLOB);


   /**
   * Returns the MD5 hash of a Clob.  Useful because dbms_obfuscation_toolkit has Varchar2 limitation.
   * This function will not work with Express editions of Oracle Database.
   *
   * %return The MD5 hash
   */
   FUNCTION md5(p_clob IN CLOB) RETURN VARCHAR2 DETERMINISTIC;
   /*
   TODO: owner="cleveridea" category="Optimize" priority="2 - Medium" created="2006-08-19"
   text="This should be conditionally compiled only if the database is a non-Express edition server."
   */


   /**
   * Same as trim() built-in, just for clobs.
   */
   FUNCTION TRIM(p_clob IN CLOB) RETURN CLOB;


   /**
   * Converts clob into associative array of strings
   *
   * %param p_clob The Clob to parse.
   * %param p_chunk_size The maximum lenght() for each element.  Max is 32000.
   */
   FUNCTION clob_to_varchars
   (
      p_clob	   IN CLOB
     ,p_chunk_size IN INTEGER
   ) RETURN dbms_sql.varchar2a;

   /** (internal) Used as element for attachment_tbl_type.
   */
   type attachment_rec_type is record(
       binary_file blob
      ,file_name   varchar2(512));

   /** Used by send for p_attachment parameter
   */
   type attachment_tbl_type is table of attachment_rec_type index by binary_integer;

   /**
   * Used in place of null in send
   */
   g_no_attachments attachment_tbl_type;

   /**
   * Used by send;2 to simplify procedural coding through recyclying a context rather than
   * rebuilding redundant parameters.
   */
   type mail_context_rec_type is record(
       from_email    varchar2(4000)
      ,from_replyto  varchar2(4000)
      ,to_list	     varchar2(4000)
      ,cc_list	     varchar2(4000)
      ,bcc_list      varchar2(4000)
      ,subject	     varchar2(1000)
      ,text_message  clob
      ,content_type  varchar2(512) default 'text/plain;charset=UTF8'
      ,attachments   attachment_tbl_type default g_no_attachments
      ,priority      varchar2(1) default '3'
      ,auth_username varchar2(512)
      ,auth_password varchar2(512)
      ,mail_server   varchar2(512)
      ,port	     integer default 25);


   /**
   * Send e-mail via smtp.  This procedure is a little bulky to be used directly for every need.  Ultimately, you
   * should build a wrapper that suits your needs, like simple_send_html.  Alternatively, you can use the overloaded
   * send;2 that makes use of a reusable context to enable code clarity in your projects.
   *
   * %param p_from_email Value that is submitted to smtp server for MAIL FROM:<reverse-path> command, not the
   *	    immediately visible From: e-mail data component seen by the recipient(s).  This should be a valid
   *	    formatted and real e-mail address that the smtp server knows and allows.
   *
   * %param p_from_replyto The visible From: component of e-mail data and always seen by recipient(s).	Also
   *	    functions as the ReplyTo: component.  This can take the form of a valid e-mail address or the
   *	    format of "Friendly Name [email@address.com]" <i>Replace '[]' with '<>' (plsqldoc limitation).</i>
   *
   * %param p_to_list Multi-functional: serves as both the To: component of e-mail data seen by all recipient(s)
   *	    as well as the the RCPT TO:<forward-path> command(s).  This can be passed as a single value or
   *	    multiple values seperated by commas or semicolons.
   *
   * %param p_cc_list Multi-functional: serves as both the Cc: component of e-mail data seen by all recipient(s)
   *	    as well as the the RCPT TO:<forward-path> command(s).  This can be passed as a single value or
   *	    multiple values seperated by commas or semicolons.
   *
   * %param p_bcc_list The Bcc: component of e-mail data seen by no recipient(s).  This can be passed as a
   *	    single value or multiple values seperated by commas or semicolons.
   *
   * %param p_subject The Subject: component of e-mail data visible to recipient(s).
   *
   * %param p_text_message The text content of the message; can be plain, html, or whatever - just provide
   *	    the appropriate p_content_type string
   *
   * %param p_content_type Gives the Mime-compliant e-mail reader a fighting chance to render the
   *	    p_text_message.
   *
   * %param p_attachments An array of binary attachments and thier names.  Supply g_no_attachments if
   *	    your message has no attachments (you <b>cannot</b> simply pass null).
   *
   * %param p_priority The very optional e-mail priority/importance which is interpreted differently by
   *	    a variety of e-mail readers.  Use a single character value of 1, 2, 3, 4, or 5.  1 is
   *	    highest importantance, and 5 is lowest importance.	3 is normal.
   *
   * %param p_auth_username The username supplied to the smtp server.  Use null if not needed.
   *
   * %param p_auth_password The password supplied to the smtp server.  Use null if not needed.
   *
   * %param p_mail_server The smtp server name.
   *
   * %param p_port The smtp server port, default is 25.
   *
   * %usage <code>
   * declare
   *	v_message clob;
   *	v_binary_attachment blob;
   *	v_attachments cio_mail.attachment_tbl_type;
   * begin
   *	v_message := '<html><head/><body style="font-weight:bold;color=red;">Some bold and red text</body></html>';
   *	v_binary_attachment := utl_raw.cast_to_raw(v_message);
   *
   *	v_attachments(1).binary_file := v_binary_attachment;
   *	v_attachments(1).file_name := 'whatever.html';
   *
   *	cio_mail.send(
   *	   p_from_email    => 'real_email@real_domain.net',
   *	   p_from_replyto  => 'Whomever [whatever@whatever.com]',  -- replace '[]' with '<>' (plsqldoc limitation)
   *	   p_to_list	   => 'target1@domain.com,target2@domain.com',
   *	   p_cc_list	   => 'target3@domain.com;target4@domain.com',
   *	   p_bcc_list	   => 'bigbrother@fbi.gov',
   *	   p_subject	   => 'Test Message @ ' || to_char(sysdate, 'HH24:MI:SS'),
   *	   p_text_message  => v_message,
   *	   p_content_type  => 'text/html;charset=UTF8',
   *	   p_attachments   => v_attachments,
   *	   p_priority	   => '3',
   *	   p_auth_username => 'account@real_domain.net',
   *	   p_auth_password => 'password',
   *	   p_mail_server   => 'mail.real_domain.net'
   *	);
   * end;</code>
   */
   procedure send
   (
      p_from_email    in varchar2
     ,p_from_replyto  in varchar2
     ,p_to_list       in varchar2
     ,p_cc_list       in VARCHAR2 DEFAULT NULL
     ,p_bcc_list      in VARCHAR2 DEFAULT NULL
     ,p_subject       in varchar2
     ,p_text_message  in clob
     ,p_content_type  in VARCHAR2 DEFAULT 'text/html;charset=UTF8'
     ,p_attachments   in attachment_tbl_type
     ,p_priority      in VARCHAR2 DEFAULT 3
     ,p_auth_username in VARCHAR2 DEFAULT NULL
     ,p_auth_password in VARCHAR2 DEFAULT NULL
     ,p_mail_server   in VARCHAR2 DEFAULT 'tyccas01.brierley.com'
     ,p_port	      in integer default 25
   );

   procedure send_external
   (
      p_from_email    in varchar2
     ,p_from_replyto  in varchar2
     ,p_to_list       in varchar2
     ,p_cc_list       in VARCHAR2 DEFAULT NULL
     ,p_bcc_list      in VARCHAR2 DEFAULT NULL
     ,p_subject       in varchar2
     ,p_text_message  in clob
     ,p_content_type  in VARCHAR2 DEFAULT 'text/html;charset=UTF8'
     ,p_attachments   in attachment_tbl_type
     ,p_priority      in VARCHAR2 DEFAULT 3
     ,p_auth_username in VARCHAR2 DEFAULT NULL
     ,p_auth_password in VARCHAR2 DEFAULT NULL
     ,p_mail_server   in VARCHAR2 DEFAULT 'tyccas01.brierley.com'
     ,p_port	      in integer default 25
     ,p_returnmessage  out VARCHAR2
   );


   /**
   * Send e-mail via smtp (overload that uses mail_context record rather than parameters).  See send;1.
   *
   * %param p_mail_context Set of values used to call send.
   *
   * %usage <code>
   * declare
   *	v_ctx cio_mail.mail_context_rec_type;
   * begin
   *	v_ctx.from_email    := 'real_email@real_domain.net';
   *	v_ctx.from_replyto  := 'Whomever [whatever@whatever.com]'; -- replace '[]' with '<>' (plsqldoc limitation)
   *	v_ctx.auth_username := 'account@real_domain.net';
   *	v_ctx.auth_password := 'password';
   *	v_ctx.mail_server   := 'mail.real_domain.net';
   *
   *	v_ctx.text_message  := 'Message for somebody.';
   *	v_ctx.to_list	    := 'somebody@something.net';
   *	v_ctx.subject	    := 'Test Message @ ' || to_char(sysdate, 'HH24:MI:SS');
   *	cio_mail.send(v_ctx);
   *
   *	v_ctx.text_message  := 'Message for somebodyelse, recycycling the context as much as possible.';
   *	v_ctx.to_list	    := 'somebodyelse@something.net;andhisbrother@something.net';
   *	v_ctx.subject	    := 'Test Message @ ' || to_char(sysdate, 'HH24:MI:SS');
   *	cio_mail.send(v_ctx);
   * end; </code>
   */
   procedure send(p_mail_context in mail_context_rec_type);



   /**
   * Send wrapper intended for a simple html content no attachment e-mail via smtp.  See send;1 for parameters
   * not illuminated specifically.
   *
   * %param p_from Your real e-mail address, nothing fancier.
   *
   * %param p_html_message Any html content.
   */
   procedure simple_send_html
   (
      p_from	      in varchar2
     ,p_to_list       in varchar2
     ,p_subject       in varchar2
     ,p_html_message  in clob
     ,p_mail_server   in VARCHAR2 DEFAULT  'tyccas01.brierley.com'
     ,p_auth_password in VARCHAR2 DEFAULT NULL
     ,p_auth_username in VARCHAR2 DEFAULT NULL
   );
   procedure simple_send_html
   (
      p_from	      in varchar2
     ,p_to_list       in VARCHAR2
     ,p_cc_list       IN VARCHAR2
     ,p_subject       in varchar2
     ,p_html_message  in clob
     ,p_mail_server   in VARCHAR2 DEFAULT  'tyccas01.brierley.com'
     ,p_auth_password in VARCHAR2 DEFAULT NULL
     ,p_auth_username in VARCHAR2 DEFAULT NULL
   );

end cio_mail;
/

create or replace package body cio_mail is

   c_x_mailer constant varchar2(256) := 'CleverIdeasForOracle http://cleveridea.net';
   g_perform_logging BOOLEAN := FALSE;


   error_stack_message_table error_stack_token_table_type;

   FUNCTION error_stack_message(p_exception IN VARCHAR2) RETURN VARCHAR2 IS
   BEGIN
      RETURN error_stack_message_table(LOWER(p_exception));
   END error_stack_message;


   PROCEDURE LOG(p_exception IN VARCHAR2) IS
      PRAGMA AUTONOMOUS_TRANSACTION;

   BEGIN
      NULL;
      INSERT INTO cio_x_log
	 (exception_thrown, application_client_info, application_module, application_action, call_stack, error_stack)
      VALUES
	 (p_exception, get_application_client_info, get_application_module, get_application_action, dbms_utility.format_call_stack, dbms_utility.format_error_stack);
      COMMIT;
   END LOG;


   -- private -------------------------------------------------------


   PROCEDURE purge_logging IS
   BEGIN
      DELETE FROM cio_x_log;
   END purge_logging;


   FUNCTION get_last_logging RETURN cio_x_log%ROWTYPE IS
      v_return cio_x_log%ROWTYPE;
   BEGIN
      SELECT t.* INTO v_return FROM cio_x_log t WHERE t.when_thrown = (SELECT MAX(t2.when_thrown) FROM cio_x_log t2);
      RETURN v_return;
   EXCEPTION
      WHEN no_data_found THEN
	 RETURN NULL;
      WHEN too_many_rows THEN
	 -- it's possible that this could occur, but when_thrown is a timestamp to the 1000th of a second
	 RAISE;
   END get_last_logging;


   PROCEDURE logging_enable IS
   BEGIN
      g_perform_logging := TRUE;
   END logging_enable;


   PROCEDURE logging_disable IS
   BEGIN
      g_perform_logging := FALSE;
   END logging_disable;


   FUNCTION logging_status RETURN BOOLEAN IS
   BEGIN
      RETURN g_perform_logging;
   END logging_status;


   FUNCTION get_application_client_info RETURN VARCHAR2 IS
      v_return VARCHAR2(4000);
   BEGIN
      dbms_application_info.set_client_info(v_return);
      RETURN v_return;
   END get_application_client_info;


   FUNCTION get_application_module RETURN VARCHAR2 IS
      v_return VARCHAR2(4000);
      v_dummy  VARCHAR2(4000);
   BEGIN
      dbms_application_info.read_module(module_name => v_return, action_name => v_dummy);
      RETURN v_return;
   END get_application_module;


   FUNCTION get_application_action RETURN VARCHAR2 IS
      v_return VARCHAR2(4000);
      v_dummy  VARCHAR2(4000);
   BEGIN
      dbms_application_info.read_module(module_name => v_dummy, action_name => v_return);
      RETURN v_return;
   END get_application_action;


   FUNCTION CODE(p_exception IN VARCHAR2) RETURN INTEGER IS
      v_return INTEGER;
   BEGIN
      BEGIN
	 EXECUTE IMMEDIATE (' begin raise ' || p_exception || '; end;');
      EXCEPTION
	 WHEN OTHERS THEN
	    v_return := SQLCODE;
      END;

      RETURN v_return;
   END CODE;


   PROCEDURE throw
   (
      p_exception IN VARCHAR2
     ,p_tokens	  IN error_stack_token_table_type
     ,p_logging   IN BOOLEAN DEFAULT logging_status
   ) IS
      v_message 		     VARCHAR2(4000);
      v_schema_package_for_exception VARCHAR2(256);
      v_tokens_for_throw_execution   error_stack_token_table_type;



      FUNCTION replace_tokens_in_message
      (
	 p_tokens  IN error_stack_token_table_type
	,p_message IN VARCHAR2
      ) RETURN VARCHAR2 IS
	 v_return VARCHAR2(4000) := p_message;
	 v_token  VARCHAR2(256);

	 FUNCTION replace_token
	 (
	    p_message		IN VARCHAR2
	   ,p_token_position	IN INTEGER
	   ,p_replacement_value IN VARCHAR2
	 ) RETURN VARCHAR2 IS
	    v_return	   VARCHAR2(256) := p_message;
	    v_match_string VARCHAR2(3) := '%' || to_char(p_token_position);
	 BEGIN
	    IF LENGTH(p_replacement_value) = 0
	    THEN
	       -- no argument / remove placeholder
	       v_match_string := ' ' || v_match_string; --leading space must also be removed
	    END IF;

	    RETURN REPLACE(v_return, v_match_string, p_replacement_value);
	 END replace_token;

	 FUNCTION count_of_tokens(p_string IN VARCHAR2) RETURN INTEGER IS
	    v_return INTEGER;
	    c_place_holder_character CONSTANT VARCHAR2(1) := '%';
	 BEGIN
	    IF p_string IS NULL
	    THEN
	       v_return := 0;
	    ELSE
	       v_return := 0;
	       FOR i IN 1 .. LENGTH(p_string)
	       LOOP
		  IF SUBSTR(p_string, i, 1) = c_place_holder_character
		  THEN
		     v_return := v_return + 1;
		  END IF;
	       END LOOP;
	    END IF;

	    RETURN v_return;
	 END count_of_tokens;

      BEGIN
	 IF p_message IS NULL
	 THEN
	    v_return := NULL;

	 ELSE
	    FOR i IN 1 .. count_of_tokens(p_message)
	    LOOP
	       BEGIN
		  v_token := p_tokens(i);
	       EXCEPTION
		  WHEN NO_DATA_FOUND THEN
		     v_token := NULL;
	       END;
	       v_return := replace_token(v_return, i, v_token);
	    END LOOP;
	 END IF;

	 RETURN v_return;
      END replace_tokens_in_message;

      FUNCTION derive_schema_package(p_exception IN VARCHAR2) RETURN VARCHAR2 IS
	 v_return VARCHAR2(256);
      BEGIN
	 RETURN SUBSTR(p_exception, 1, INSTR(p_exception, '.', -1) - 1);
      END derive_schema_package;

   BEGIN
      BEGIN
	 BEGIN
	    EXECUTE IMMEDIATE ('begin raise ' || p_exception || '; end;');
	    -- exception is raised and immediately trapped
	 EXCEPTION
	    WHEN plsql_compilation_error THEN
	       v_tokens_for_throw_execution(1) := '"' || p_exception || '"';
	       throw("exception does not exist", v_tokens_for_throw_execution);
	       -- note: if "exception does not exist" does not exist very undesirable recursive exception handling occurs
	 END;

      EXCEPTION
	 WHEN OTHERS THEN
	    IF SQLCODE NOT BETWEEN - 20999 AND - 20000
	    THEN
	       RAISE;
	       -- nothing extra to do for extra for system errors, this raise is also immmediately trapped
	    ELSE
	       DECLARE
		  cmd VARCHAR2(4000);
	       BEGIN
		  cmd := 'select ' || derive_schema_package(p_exception) || '.error_stack_message(''' || UPPER(p_exception) || ''') from dual';
		  EXECUTE IMMEDIATE (cmd)
		     INTO v_message;
	       EXCEPTION
		  WHEN plsql_compilation_error OR missing_expresion THEN
		     v_message := NULL;
		  WHEN no_data_found THEN
		     v_message := NULL;
	       END;

	       raise_application_error(SQLCODE, replace_tokens_in_message(p_tokens, v_message));
	       -- integrates error stack message API, this raise is also immediately trapped

	    END IF;
      END;
   EXCEPTION
      WHEN OTHERS THEN
	 IF p_logging
	 THEN
	    LOG(p_exception);
	    -- writes to cio_x_log table
	 END IF;

	RAISE; -- finally, bubbles up the original throw() call
   END Throw;

   PROCEDURE sanity_check_on_seperator
   (
      p_separator IN VARCHAR2
     ,v_tokens	  OUT error_stack_token_table_type
   ) IS
   BEGIN
      IF p_separator IS NULL
      THEN
	 v_tokens(1) := 'p_separator';
	 RAISE parameter_cannot_be_null;
      END IF;

      IF LENGTH(p_separator) > 1
      THEN
	 v_tokens(1) := 'p_seperator';
	 v_tokens(2) := 'of a single character';
	 RAISE parameter_did_not_conform;
      END IF;
   END sanity_check_on_seperator;


   FUNCTION list_to_table
   (
      p_list	  IN VARCHAR2
     ,p_separator IN VARCHAR2 DEFAULT ','
   ) RETURN cio_plsql_string_array IS

      c_max_elements CONSTANT BINARY_INTEGER := 16384;

      v_tokens error_stack_token_table_type;

      v_return			cio_plsql_string_array := NEW cio_plsql_string_array();
      v_element_count		BINARY_INTEGER;
      v_previous_comma_position BINARY_INTEGER;
      v_next_comma_position	BINARY_INTEGER;

   BEGIN
      sanity_check_on_seperator(p_separator, v_tokens);

      IF p_list IS NULL
      THEN
	 RETURN NULL;
      END IF;

      v_previous_comma_position := 0;
      v_next_comma_position	:= 0;
      v_element_count		:= 1;

      FOR i IN 1 .. c_max_elements
      LOOP
	 v_next_comma_position := INSTR(p_list, p_separator, v_previous_comma_position + 1, 1);

	 IF v_next_comma_position = 0
	 THEN
	    EXIT;
	 END IF;

	 v_previous_comma_position := v_next_comma_position;
	 v_element_count	   := v_element_count + 1;

      END LOOP;

      v_previous_comma_position := 0;
      v_next_comma_position	:= 0;

      FOR i IN 1 .. v_element_count - 1
      LOOP
	 v_return.extend;

	 v_next_comma_position := INSTR(p_list, p_separator, 1, i);

	 dbms_output.put_line('v_next_comma_position	 =' || v_next_comma_position);
	 dbms_output.put_line('v_previous_comma_position =' || v_previous_comma_position);

	 v_return(i) := SUBSTR(p_list, v_previous_comma_position + 1, v_next_comma_position - v_previous_comma_position - 1);

	 v_previous_comma_position := v_next_comma_position;

      END LOOP;

      -- get the last element
      IF v_element_count > 1
      THEN
	 v_return.extend;
	 v_return(v_element_count) := SUBSTR(p_list, v_next_comma_position + 1);
      END IF;

      RETURN v_return;

   EXCEPTION
      WHEN parameter_did_not_conform THEN
	 throw("parameter did not conform", v_tokens);

      WHEN parameter_cannot_be_null THEN
	 throw("parameter cannot be null", v_tokens);

   END list_to_table;


   FUNCTION table_to_list
   (
      p_table	  IN cio_plsql_string_array
     ,p_separator IN VARCHAR2 DEFAULT ','
   ) RETURN VARCHAR2 IS
      v_return VARCHAR2(32767);
      v_tokens error_stack_token_table_type;
   BEGIN
      sanity_check_on_seperator(p_separator, v_tokens);

      IF p_table IS NULL
      THEN
	 RETURN NULL;
      END IF;

      IF p_table.count = 0
      THEN
	 RETURN NULL;
      END IF;

      FOR i IN 1 .. p_table.count
      LOOP
	 v_return := v_return || p_separator || p_table(i);
      END LOOP;

      v_return := LTRIM(v_return, p_separator);

      RETURN v_return;

   EXCEPTION
      WHEN parameter_did_not_conform THEN
	 throw("parameter did not conform", v_tokens);

      WHEN parameter_cannot_be_null THEN
	 throw("parameter cannot be null", v_tokens);

   END table_to_list;

  FUNCTION CLOB_to_blob (C IN CLOB)
    RETURN BLOB
      IS
	V_CLOB	  CLOB:=C;
	V_BLOB	  BLOB;
	V_IN	  PLS_INTEGER := 1;
	V_OUT	  PLS_INTEGER := 1;
	V_LANG	  PLS_INTEGER := 0;
	V_WARNING PLS_INTEGER := 0;
	V_ID	  NUMBER(10);
    BEGIN
		V_IN   := 1;
		V_OUT  := 1;
		DBMS_LOB.CREATETEMPORARY(V_BLOB, TRUE);
		DBMS_LOB.CONVERTTOBLOB(V_BLOB,
							      V_CLOB,
							      DBMS_LOB.GETLENGTH(V_CLOB),
							      V_IN,
							      V_OUT,
							      DBMS_LOB.DEFAULT_CSID,
							      V_LANG,
							      V_WARNING);
	RETURN v_blob;
	END clob_to_blob;

   PROCEDURE tidy_up_chunks(v_array IN OUT dbms_sql.varchar2a) IS
   BEGIN
      FOR i IN 1 .. v_array.count
      LOOP
	 v_array(i) := REPLACE(v_array(i), CHR(13));
	 -- chr(13)'s introduced by httpuritype.getclob() and must be removed
      END LOOP;

      v_array(v_array.last) := RTRIM(v_array(v_array.last), CHR(10) || ' ');
      v_array(v_array.last) := RTRIM(v_array(v_array.last), CHR(10) || '/');

   END tidy_up_chunks;


   FUNCTION clob_to_varchars
   (
      p_clob	   IN CLOB
     ,p_chunk_size IN INTEGER
   ) RETURN dbms_sql.varchar2a IS
      v_result	      dbms_sql.varchar2a;
      v_cursor_id     INTEGER;
      v_cursor_result INTEGER;
   BEGIN
      FOR i IN 0 .. TRUNC(dbms_lob.getlength(p_clob) / p_chunk_size)
      LOOP
	 v_result(i + 1) := dbms_lob.substr(p_clob, p_chunk_size, (i * p_chunk_size) + 1);
      END LOOP;

      RETURN v_result;
   END clob_to_varchars;


   FUNCTION TRIM(p_clob IN CLOB) RETURN CLOB IS
      c_whitespace CONSTANT VARCHAR2(4) := ' ' || CHR(9) || CHR(10) || CHR(13);
      RESULT CLOB;
   BEGIN
      RESULT := LTRIM(RTRIM(p_clob, c_whitespace), c_whitespace); -- trim doesn't work well with crlf
      RETURN RESULT;
   END TRIM;


   PROCEDURE execute_plsql(p_clob IN CLOB) IS
      v_chunks	      dbms_sql.varchar2a;
      v_chunk_size    INTEGER := 256;
      v_cursor_id     INTEGER;
      v_cursor_result INTEGER;
   BEGIN

      v_chunks := clob_to_varchars(trim(p_clob), 128); -- special trim required because leading whitespace causes ora-00911 invalid character...
      tidy_up_chunks(v_chunks);

      v_cursor_id := dbms_sql.open_cursor;

      dbms_sql.parse(v_cursor_id, v_chunks, v_chunks.first, v_chunks.last, FALSE, dbms_sql.native);

      v_cursor_result := dbms_sql.execute(v_cursor_id);

      dbms_sql.close_cursor(v_cursor_id);

   END execute_plsql;


   /*
   TODO: owner="cleveridea" category="Optimize" priority="2 - Medium" created="2006-08-19"
   text="This should be conditionally compiled only if the database is a non-Express edition server."
   */
   FUNCTION md5(p_clob IN CLOB) RETURN VARCHAR2 DETERMINISTIC AS
      LANGUAGE JAVA NAME 'net.cleveridea.cio_md5lob.hash(oracle.sql.CLOB) return String';

   procedure smtp_write
   (
      c    in out nocopy utl_smtp.connection
     ,data in varchar2
   );

   procedure add_attachment
   (
      v_connection	in out nocopy utl_smtp.connection
     ,p_attachment	in out blob
     ,p_attachment_name in varchar2
     ,p_boundary	in varchar2
   );


   procedure send_external
   (
      p_from_email    in varchar2
     ,p_from_replyto  in varchar2
     ,p_to_list       in varchar2
     ,p_cc_list       in VARCHAR2 DEFAULT NULL
     ,p_bcc_list      in VARCHAR2 DEFAULT NULL
     ,p_subject       in varchar2
     ,p_text_message  in clob
     ,p_content_type  in VARCHAR2 DEFAULT 'text/html;charset=UTF8'
     ,p_attachments   in attachment_tbl_type
     ,p_priority      in VARCHAR2 DEFAULT 3
     ,p_auth_username in VARCHAR2 DEFAULT NULL
     ,p_auth_password in VARCHAR2 DEFAULT NULL
     ,p_mail_server   in VARCHAR2 DEFAULT 'tyccas01.brierley.com'
     ,p_port	      in integer default 25
     ,p_returnmessage  out VARCHAR2
   ) is
      c_default_content_type constant varchar2(512) := 'text/plain';

      v_connection   utl_smtp.connection;
      v_chunks	     dbms_sql.varchar2a;
      v_blob	     blob;
      v_boundary     varchar2(80) := 'CleverIdeasForOracleBoundary';
      v_from_replyto varchar2(4000);
      v_content_type varchar2(512);
      v_this_chunk   VARCHAR2(30000);
      procedure send_rcpt(p_target in varchar2) is
	 v_rcpt_array cio_plsql_string_array;
      begin
	 if (instr(replace(p_target, ';', ','), ',') > 0)
	 then
	    v_rcpt_array := list_to_table(replace(p_target, ';', ','));
	    for i in 1 .. v_rcpt_array.count
	    loop
	       utl_smtp.rcpt(v_connection, v_rcpt_array(i));
	    end loop;
	 else
	    utl_smtp.rcpt(v_connection, p_target);
	 end if;
      end send_rcpt;

   begin

      v_connection := utl_smtp.open_connection(p_mail_server, p_port);

      utl_smtp.ehlo(v_connection, p_mail_server);

      if (p_auth_username is not null or p_auth_password is not null)
      then
	 utl_smtp.command(v_connection, 'AUTH LOGIN');
	 utl_smtp.command(v_connection, utl_raw.cast_to_varchar2(utl_encode.base64_encode(utl_raw.cast_to_raw(p_auth_username))));
	 utl_smtp.command(v_connection, utl_raw.cast_to_varchar2(utl_encode.base64_encode(utl_raw.cast_to_raw(p_auth_password))));
      end if;

      utl_smtp.mail(v_connection, p_from_email);

      if trim(p_to_list) is not null
      then
	 send_rcpt(trim(p_to_list));
      end if;

      if trim(p_cc_list) is not null
      then
	 send_rcpt(trim(p_cc_list));
      end if;

      if trim(p_bcc_list) is not null
      then
	 send_rcpt(trim(p_bcc_list));
      end if;

      utl_smtp.open_data(v_connection);

      v_from_replyto := nvl(trim(p_from_replyto), trim(p_from_email));
      if (v_from_replyto is not null)
      then
	 smtp_write(v_connection, 'From:' || v_from_replyto);
	 smtp_write(v_connection, 'ReplyTo:' || v_from_replyto);
	 -- these are coupled because for the e-mail clients I tested, the ReplyTo:
	 -- was simply ignored and the From: was used anyway.
      end if;

      if (trim(p_to_list) is not null)
      then
	 smtp_write(v_connection, 'To:' || trim(p_to_list));
      end if;

      if (trim(p_cc_list) is not null)
      then
	 smtp_write(v_connection, 'Cc:' || trim(p_cc_list));
      end if;

      if (trim(p_subject) is not null)
      then
	 smtp_write(v_connection, 'Subject:' || trim(p_subject));
      end if;

      smtp_write(v_connection, 'X-Mailer:' || c_x_mailer);

      if (trim(p_priority) is not null)
      then
	 smtp_write(v_connection, 'X-Priority:' || trim(p_priority));
      end if;

      if p_attachments.count > 0
      then
	 smtp_write(v_connection, 'Content-Type:multipart/mixed;boundary=' || v_boundary);
	 smtp_write(v_connection, '');
	 smtp_write(v_connection, 'You are reading this because you are ');
	 smtp_write(v_connection, 'using non-MIME compliant reader - this mail agent ');
	 smtp_write(v_connection, c_x_mailer || ' expects you to be using one.');
	 smtp_write(v_connection, '');
	 smtp_write(v_connection, '--' || v_boundary);
      end if;

      v_content_type := nvl(trim(p_content_type), c_default_content_type);
      smtp_write(v_connection, 'Content-Type:' || v_content_type);
      smtp_write(v_connection, '');

      if (nvl(length(p_text_message), 0) > 0)
      then
	 v_chunks := clob_to_varchars(p_text_message, 900);
	 for i in v_chunks.first .. v_chunks.last
	 LOOP
	    --smtp_write(v_connection, replace(v_chunks(i), chr(10), chr(13)));
	    v_this_chunk := replace(v_chunks(i), chr(10), ' ');
	    v_this_chunk := replace(v_chunks(i), chr(13), ' ');
	    smtp_write(v_connection, v_this_chunk);
	 end loop;
      end if;

      smtp_write(v_connection, '');
      smtp_write(v_connection, '');

      if p_attachments.count > 0
      then

	 for i in p_attachments.first .. p_attachments.last
	 loop
	    v_blob := p_attachments(i).binary_file;
	    add_attachment(v_connection, v_blob, p_attachments(i).file_name, v_boundary);
	 end loop;

      end if;

      smtp_write(v_connection, '');

      if p_attachments.count > 0
      then
	 smtp_write(v_connection, '--' || v_boundary || '--');
      end if;

      utl_smtp.close_data(v_connection);

      utl_smtp.quit(v_connection);
      p_returnmessage := 'SUCCESS';

   exception
      when utl_smtp.transient_error or utl_smtp.permanent_error then
	 begin
	    utl_smtp.quit(v_connection);
	 exception
	    when utl_smtp.transient_error or utl_smtp.permanent_error then
	       null; -- the quit call will raise an exception that we can ignore.
	 end;
	 p_returnmessage := SQLERRM;
      when others then
	 p_returnmessage := SQLERRM;
   end send_external;

procedure send
   (
      p_from_email    in varchar2
     ,p_from_replyto  in varchar2
     ,p_to_list       in varchar2
     ,p_cc_list       in VARCHAR2 DEFAULT NULL
     ,p_bcc_list      in VARCHAR2 DEFAULT NULL
     ,p_subject       in varchar2
     ,p_text_message  in clob
     ,p_content_type  in VARCHAR2 DEFAULT 'text/html;charset=UTF8'
     ,p_attachments   in attachment_tbl_type
     ,p_priority      in VARCHAR2 DEFAULT 3
     ,p_auth_username in VARCHAR2 DEFAULT NULL
     ,p_auth_password in VARCHAR2 DEFAULT NULL
     ,p_mail_server   in VARCHAR2 DEFAULT 'tyccas01.brierley.com'
     ,p_port	      in integer default 25
   ) is
      c_default_content_type constant varchar2(512) := 'text/plain';

      v_connection   utl_smtp.connection;
      v_chunks	     dbms_sql.varchar2a;
      v_blob	     blob;
      v_boundary     varchar2(80) := 'CleverIdeasForOracleBoundary';
      v_from_replyto varchar2(4000);
      v_content_type varchar2(512);
      v_this_chunk   VARCHAR2(30000);
      procedure send_rcpt(p_target in varchar2) is
	 v_rcpt_array cio_plsql_string_array;
      begin
	 if (instr(replace(p_target, ';', ','), ',') > 0)
	 then
	    v_rcpt_array := list_to_table(replace(p_target, ';', ','));
	    for i in 1 .. v_rcpt_array.count
	    loop
	       utl_smtp.rcpt(v_connection, v_rcpt_array(i));
	    end loop;
	 else
	    utl_smtp.rcpt(v_connection, p_target);
	 end if;
      end send_rcpt;

   begin

      v_connection := utl_smtp.open_connection(p_mail_server, p_port);

      utl_smtp.ehlo(v_connection, p_mail_server);

      if (p_auth_username is not null or p_auth_password is not null)
      then
	 utl_smtp.command(v_connection, 'AUTH LOGIN');
	 utl_smtp.command(v_connection, utl_raw.cast_to_varchar2(utl_encode.base64_encode(utl_raw.cast_to_raw(p_auth_username))));
	 utl_smtp.command(v_connection, utl_raw.cast_to_varchar2(utl_encode.base64_encode(utl_raw.cast_to_raw(p_auth_password))));
      end if;

      utl_smtp.mail(v_connection, p_from_email);

      if trim(p_to_list) is not null
      then
	 send_rcpt(trim(p_to_list));
      end if;

      if trim(p_cc_list) is not null
      then
	 send_rcpt(trim(p_cc_list));
      end if;

      if trim(p_bcc_list) is not null
      then
	 send_rcpt(trim(p_bcc_list));
      end if;

      utl_smtp.open_data(v_connection);

      v_from_replyto := nvl(trim(p_from_replyto), trim(p_from_email));
      if (v_from_replyto is not null)
      then
	 smtp_write(v_connection, 'From:' || v_from_replyto);
	 smtp_write(v_connection, 'ReplyTo:' || v_from_replyto);
	 -- these are coupled because for the e-mail clients I tested, the ReplyTo:
	 -- was simply ignored and the From: was used anyway.
      end if;

      if (trim(p_to_list) is not null)
      then
	 smtp_write(v_connection, 'To:' || trim(p_to_list));
      end if;

      if (trim(p_cc_list) is not null)
      then
	 smtp_write(v_connection, 'Cc:' || trim(p_cc_list));
      end if;

      if (trim(p_subject) is not null)
      then
	 smtp_write(v_connection, 'Subject:' || trim(p_subject));
      end if;

      smtp_write(v_connection, 'X-Mailer:' || c_x_mailer);

      if (trim(p_priority) is not null)
      then
	 smtp_write(v_connection, 'X-Priority:' || trim(p_priority));
      end if;

      if p_attachments.count > 0
      then
	 smtp_write(v_connection, 'Content-Type:multipart/mixed;boundary=' || v_boundary);
	 smtp_write(v_connection, '');
	 smtp_write(v_connection, 'You are reading this because you are ');
	 smtp_write(v_connection, 'using non-MIME compliant reader - this mail agent ');
	 smtp_write(v_connection, c_x_mailer || ' expects you to be using one.');
	 smtp_write(v_connection, '');
	 smtp_write(v_connection, '--' || v_boundary);
      end if;

      v_content_type := nvl(trim(p_content_type), c_default_content_type);
      smtp_write(v_connection, 'Content-Type:' || v_content_type);
      smtp_write(v_connection, '');

      if (nvl(length(p_text_message), 0) > 0)
      then
	 v_chunks := clob_to_varchars(p_text_message, 900);
	 for i in v_chunks.first .. v_chunks.last
	 LOOP
	    --smtp_write(v_connection, replace(v_chunks(i), chr(10), chr(13)));
	    v_this_chunk := replace(v_chunks(i), chr(10), ' ');
	    v_this_chunk := replace(v_chunks(i), chr(13), ' ');
	    smtp_write(v_connection, v_this_chunk);
	 end loop;
      end if;

      smtp_write(v_connection, '');
      smtp_write(v_connection, '');

      if p_attachments.count > 0
      then

	 for i in p_attachments.first .. p_attachments.last
	 loop
	    v_blob := p_attachments(i).binary_file;
	    add_attachment(v_connection, v_blob, p_attachments(i).file_name, v_boundary);
	 end loop;

      end if;

      smtp_write(v_connection, '');

      if p_attachments.count > 0
      then
	 smtp_write(v_connection, '--' || v_boundary || '--');
      end if;

      utl_smtp.close_data(v_connection);

      utl_smtp.quit(v_connection);
   exception
      when utl_smtp.transient_error or utl_smtp.permanent_error then
	 begin
	    utl_smtp.quit(v_connection);
	 exception
	    when utl_smtp.transient_error or utl_smtp.permanent_error then
	       null; -- the quit call will raise an exception that we can ignore.
	 end;
	 raise;
      when others then
	 raise;
   end send;

   procedure send(p_mail_context in mail_context_rec_type) is
   begin
      send(p_from_email    => p_mail_context.from_email
	  ,p_from_replyto  => p_mail_context.from_replyto
	  ,p_to_list	   => p_mail_context.to_list
	  ,p_cc_list	   => p_mail_context.cc_list
	  ,p_bcc_list	   => p_mail_context.bcc_list
	  ,p_subject	   => p_mail_context.subject
	  ,p_text_message  => p_mail_context.text_message
	  ,p_content_type  => p_mail_context.content_type
	  ,p_attachments   => p_mail_context.attachments
	  ,p_priority	   => p_mail_context.priority
	  ,p_auth_username => p_mail_context.auth_username
	  ,p_auth_password => p_mail_context.auth_password
	  ,p_mail_server   => p_mail_context.mail_server
	  ,p_port	   => p_mail_context.port);
   end send;


   procedure simple_send_html
   (
      p_from	      in varchar2
     ,p_to_list       in VARCHAR2
     ,p_cc_list       IN VARCHAR2
     ,p_subject       in varchar2
     ,p_html_message  in clob
     ,p_mail_server   in VARCHAR2 DEFAULT  'tyccas01.brierley.com'
     ,p_auth_password in VARCHAR2 DEFAULT NULL
     ,p_auth_username in VARCHAR2 DEFAULT NULL
   ) is
      mail_context mail_context_rec_type;
   begin
      mail_context.from_email	 := p_from;
      mail_context.cc_list	 := p_cc_list;
      mail_context.from_replyto  := p_from;
      mail_context.to_list	 := p_to_list;
      mail_context.subject	 := p_subject;
      mail_context.text_message  := p_html_message;
      mail_context.mail_server	 := p_mail_server;
      mail_context.auth_username := p_auth_username;
      mail_context.auth_password := p_auth_password;
      mail_context.content_type  := 'text/html;charset=UTF8';
      send(mail_context);
   end simple_send_html;

   procedure simple_send_html
   (
      p_from	      in varchar2
     ,p_to_list       in VARCHAR2
     ,p_subject       in varchar2
     ,p_html_message  in clob
     ,p_mail_server   in VARCHAR2 DEFAULT  'tyccas01.brierley.com'
     ,p_auth_password in VARCHAR2 DEFAULT NULL
     ,p_auth_username in VARCHAR2 DEFAULT NULL
   ) is
      mail_context mail_context_rec_type;
   begin
      mail_context.from_email	 := p_from;
      mail_context.from_replyto  := p_from;
      mail_context.to_list	 := p_to_list;
      mail_context.subject	 := p_subject;
      mail_context.text_message  := p_html_message;
      mail_context.mail_server	 := p_mail_server;
      mail_context.auth_username := p_auth_username;
      mail_context.auth_password := p_auth_password;

      send(mail_context);
   end simple_send_html;

   procedure add_attachment
   (
      v_connection	in out nocopy utl_smtp.connection
     ,p_attachment	in out blob
     ,p_attachment_name in varchar2
     ,p_boundary	in varchar2
   ) is
      C_BASE64_CHUNK_SIZE constant pls_integer := 78;

      v_base64_chunk	  raw(32767);
      v_position	  integer := 1;
      v_attachment_length integer;
      v_amount		  binary_integer := 32767;
   begin
      smtp_write(v_connection, '');
      smtp_write(v_connection, '');
      smtp_write(v_connection, '--' || p_boundary);
      smtp_write(v_connection, 'Content-Type: application/octet-stream; name="' || p_attachment_name || '"');
      smtp_write(v_connection, 'Content-Disposition: attachment; filename="' || p_attachment_name || '"');
      smtp_write(v_connection, 'Content-Transfer-Encoding: base64');
      smtp_write(v_connection, '');
      smtp_write(v_connection, '');

      v_attachment_length := dbms_lob.getlength(p_attachment);

      if dbms_lob.isopen(p_attachment) = 0
      then
	 dbms_lob.open(p_attachment, dbms_lob.lob_readonly);
      end if;

      while v_position < v_attachment_length
      loop
	 v_amount := C_BASE64_CHUNK_SIZE;
	 dbms_lob.read(p_attachment, v_amount, v_position, v_base64_chunk);
	 utl_smtp.write_raw_data(v_connection, utl_encode.base64_encode(v_base64_chunk));
	 v_position := v_position + C_BASE64_CHUNK_SIZE;
	 smtp_write(v_connection, '');
      end loop;

      if dbms_lob.isopen(p_attachment) != 0
      then
	 dbms_lob.close(p_attachment);
      end if;

      smtp_write(v_connection, '');
      smtp_write(v_connection, '');

   end add_attachment;

   procedure smtp_write
   (
      c    in out utl_smtp.connection
     ,data in varchar2
   ) is
   begin
      utl_smtp.write_data(c, data || utl_tcp.crlf);
   end smtp_write;

BEGIN

   -- format follows form:
   -- error_stack_message_table("constant in specification") := 'error message %1 that is still meaningful with null for every token %2';
   -- Warning: Associative array index value below MUST be lower case.

   error_stack_message_table("collection is not one based") := 'an associative array or nested table %1 must be one-based in this context %2';
   error_stack_message_table("exception does not exist") := 'the exception %1 does not exist';
   error_stack_message_table("exception outside user range") := 'the exception %1 does not exist in the user define-able range';
   error_stack_message_table("fatal html syntax error") := 'the PL/SQL Gateway detected a fatal HTML syntax error %1';
   error_stack_message_table("generic exception") := 'should not occur in production software %1';
   error_stack_message_table("invalid user") := 'the user %1 is invalid in this context %2';
   error_stack_message_table("lookup table is corrupt") := 'a lookup table %1 has data integrity violations in this context %2';
   error_stack_message_table("mandatory decision tree failed") := 'the decision tree failed to make a mandatory choice for %1';
   error_stack_message_table("package state undefined") := 'the package variable/constant %1 doesn''t exist or is unexpectedly NULL';
   error_stack_message_table("parameter cannot be null") := 'an unhandled null parameter %1 was passed in this context %2';
   error_stack_message_table("parameter did not conform") := 'the parameter value %1 did not conform to the expected format %2 in this context %3';
   error_stack_message_table("security violation") := 'there was security violation of %1 in this context %2';
   error_stack_message_table("sparse collection not allowed") := 'a sparse associative array or nested table %1 is not allowed in this context %2';
   error_stack_message_table("string too large") := 'string %1 length exceeded limit in this context %2';


end cio_mail;
/

