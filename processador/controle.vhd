library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_ARITH.ALL;
use IEEE.STD_LOGIC_UNSIGNED.ALL;


entity CONTROLE is
    Generic (
				p_DATA_WIDTH	: INTEGER := 16
    );	
    Port ( 
				i_CLK			: in  STD_LOGIC;
				i_RST			: in  STD_LOGIC;
				i_DATA   		: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-1) DOWNTO 0);	  
				i_DATA_ULA		: in  STD_LOGIC;
				o_SEL_ULA		: out STD_LOGIC_VECTOR(2 DOWNTO 0);	
				o_SEL_IMED		: out STD_LOGIC_VECTOR(1 DOWNTO 0);	
				o_WR_BCO      	: out STD_LOGIC;
				o_WR_RAM      	: out STD_LOGIC;
				o_WR_IO       	: out STD_LOGIC;
				o_RD_IO			: out STD_LOGIC;
				o_SEL_JMP		: out STD_LOGIC_VECTOR(1 DOWNTO 0);
				o_PUSH  		: out STD_LOGIC;
				o_POP   		: out STD_LOGIC;
				o_RET			: out STD_LOGIC;
				o_EN_ROM		: out STD_LOGIC;
				o_ADD_ROM		: out STD_LOGIC_VECTOR((p_DATA_WIDTH-8) DOWNTO 0);
				i_NEXT_PC   	: in  STD_LOGIC_VECTOR((p_DATA_WIDTH-8) DOWNTO 0);
				o_INT_ADD		: out STD_LOGIC_VECTOR(1 DOWNTO 0);
				i_INT0		    : in  STD_LOGIC;
				i_INT1		    : in  STD_LOGIC;
				i_INT2		    : in  STD_LOGIC
	 );
end CONTROLE;

architecture Behavioral of CONTROLE is
-----------------------------------------------------------------

	type w_State_Type is (st_FETCH, st_DECODE, st_EXECUTE, st_WAIT, st_RET, st_STOP, st_CALL, st_INPUT, 
	                      st_JMPI, st_JMPZ, st_JMPE, st_CMP, st_CALL_INT); 

	attribute syn_encoding : string;
	attribute syn_encoding of w_State_Type : type is "safe";
 
	signal w_STATE : w_State_Type;
	
	constant NOP 		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"0";
	constant LDI 		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"1";
	constant ADD 		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"2";
	constant SUB 		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"3";
	constant IO_OUT 	: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"4";
	constant IO_IN		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"5";
	constant JI 		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"6";
	constant LD 		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"7";
	constant STO 		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"8";
	constant JZ 		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"9";
	constant JE 		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"A";
	constant LG_AND		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"B";
	constant LG_OR		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"C";
	constant LG_XOR		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"D";
	constant LG_NOT		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"E";
	constant CALL 		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '0' & x"F"; 
	constant RETI		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '1' & x"0";
	constant STOP		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '1' & x"2";
	constant RET		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '1' & x"3";
	constant CMP		: STD_LOGIC_VECTOR(4 DOWNTO 0) := '1' & x"4";
	
	signal w_OPCODE		: STD_LOGIC_VECTOR(4 DOWNTO 0);
	signal w_PC     	: STD_LOGIC_VECTOR((p_DATA_WIDTH-8) DOWNTO 0);
	signal w_INTERRUPT  : STD_LOGIC;
	
	
-----------------------------------------------------------------	
begin
	
	w_INTERRUPT <= i_INT0 OR i_INT1 OR i_INT2;
	

	w_OPCODE  <= i_DATA((p_DATA_WIDTH-1) downto (p_DATA_WIDTH-5));
	o_ADD_ROM <= w_PC;
	
	
	U_MACHINE : process(i_CLK, i_RST)      
		variable v_COMP : integer := 0;
		
	begin    																						
		if (i_RST = '1') then			
			o_SEL_ULA 	<= (OTHERS => '0');
			o_SEL_IMED	<= (OTHERS => '0');
			w_PC		<= (OTHERS => '0');
			o_WR_BCO    <= '0';
			o_WR_RAM    <= '0';
			o_WR_IO    	<= '0';
			o_RD_IO		<= '0';
			o_EN_ROM	<= '1';
			o_PUSH      <= '0';
			o_POP		<= '0';
			o_RET		<= '0';
			v_COMP		:= 0;
			o_INT_ADD	<= (OTHERS => '0');
			w_STATE		<= st_WAIT;				
			
		elsif falling_edge (i_CLK) then														
			case w_STATE is	
					when st_WAIT => 
						if (i_RST = '0') then
							w_STATE <= st_FETCH;
						else	
							w_STATE <= st_WAIT;
						end if;
						
					when st_FETCH =>
						o_WR_BCO    <= '0';
						o_RET		<= '0';
						o_EN_ROM	<= '0';
						
						if (w_INTERRUPT = '1') then
							o_SEL_ULA  	<= "000";
							o_SEL_IMED 	<= "00";
							o_WR_BCO   	<= '0';
							o_WR_RAM   	<= '0';
							o_WR_IO	 	<= '0';
							o_RD_IO    	<= '0';
							o_PUSH     	<= '1';
							o_POP 	   	<= '0';
							o_RET		<= '0';
							w_STATE 	<= st_CALL_INT;
						else
							w_STATE		<= st_DECODE;
						end if;

					when st_DECODE =>
							if(w_OPCODE = STOP) then
								o_WR_BCO   	<= '0';
								o_WR_RAM   	<= '0';
								o_WR_IO	   	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE    	<= st_STOP;
								
							elsif(w_OPCODE = LDI) then
								o_SEL_ULA  	<= "000";
								o_SEL_IMED 	<= "00";
								o_WR_BCO   	<= '1';
								o_WR_RAM   	<= '0';
								o_WR_IO	   	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_EXECUTE;					
								
							elsif(w_OPCODE = ADD) then
								o_SEL_ULA  	<= "000";
								o_SEL_IMED 	<= "11";
								o_WR_BCO   	<= '1';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_EXECUTE;					
								
							elsif(w_OPCODE = SUB) then
								o_SEL_ULA  	<= "001";
								o_SEL_IMED 	<= "11";
								o_WR_BCO   	<= '1';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_EXECUTE;					
								
							elsif(w_OPCODE = IO_OUT) then
								o_SEL_ULA  	<= "000";
								o_SEL_IMED 	<= "00";
								o_WR_BCO   	<= '0';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '1';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_EXECUTE;					
								
							elsif(w_OPCODE = IO_IN) then
								o_SEL_ULA  	<= "000";
								o_SEL_IMED 	<= "01";
								o_WR_BCO   	<= '0';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '1';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_INPUT;					
								
							elsif(w_OPCODE = JI) then
								o_SEL_ULA  	<= "000";
								o_SEL_IMED 	<= "00";
								o_WR_BCO   	<= '0';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_PC		<= i_DATA((p_DATA_WIDTH-8) DOWNTO 0);
								w_STATE 	<= st_JMPI;					
								
							elsif(w_OPCODE = LD) then
								o_SEL_ULA  	<= "000";
								o_SEL_IMED 	<= "10";
								o_WR_BCO   	<= '1';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_EXECUTE;					
								
							elsif(w_OPCODE = STO) then
								o_SEL_ULA  	<= "000";
								o_SEL_IMED 	<= "00";
								o_WR_BCO   	<= '0';
								o_WR_RAM   	<= '1';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_SEL_JMP  	<= "00";
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_EXECUTE;					
								
							elsif(w_OPCODE = JZ) then
								o_SEL_ULA  	<= "111";
								o_SEL_IMED 	<= "00";
								o_WR_BCO   	<= '0';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE		<= st_JMPZ;
																					
								
							elsif(w_OPCODE = JE) then
								o_SEL_ULA  	<= "110";
								o_SEL_IMED 	<= "00";
								o_WR_BCO   	<= '0';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_JMPE;
										
							elsif(w_OPCODE = CMP) then
								o_SEL_ULA  	<= "110";
								o_SEL_IMED 	<= "00";
								o_WR_BCO   	<= '0';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_CMP;
																		
							elsif(w_OPCODE = LG_AND) then
								o_SEL_ULA  	<= "010";
								o_SEL_IMED 	<= "11";
								o_WR_BCO   	<= '1';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_EXECUTE;					
								
							elsif(w_OPCODE = LG_OR) then
								o_SEL_ULA  	<= "011";
								o_SEL_IMED 	<= "11";
								o_WR_BCO   	<= '1';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_EXECUTE;					

							elsif(w_OPCODE = LG_XOR) then
								o_SEL_ULA  	<= "100";
								o_SEL_IMED 	<= "11";
								o_WR_BCO   	<= '1';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_EXECUTE;					
								
							elsif(w_OPCODE = LG_NOT) then
								o_SEL_ULA  	<= "101";
								o_SEL_IMED 	<= "11";
								o_WR_BCO   	<= '1';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_EXECUTE;					
															
							elsif(w_OPCODE = CALL) then
								o_SEL_ULA  	<= "000";
								o_SEL_IMED 	<= "00";
								o_WR_BCO   	<= '0';
								o_WR_RAM   	<= '0';
								o_WR_IO	 	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '1';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_CALL;					

								
							elsif(w_OPCODE = RET) then
								o_SEL_ULA  	<= "000";
								o_SEL_IMED 	<= "00";
								o_WR_BCO   	<= '0';
								o_WR_RAM   	<= '0';
								o_WR_IO	   	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '1';
								o_RET		<= '0';
								w_STATE 	<= st_RET;					

								
							elsif(w_OPCODE = RETI) then
								o_SEL_ULA  	<= "000";
								o_SEL_IMED 	<= "00";
								o_WR_BCO   	<= '0';
								o_WR_RAM   	<= '0';
								o_WR_IO	   	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '1';
								o_RET		<= '0';
								o_INT_ADD	<= "00";
								w_STATE 	<= st_RET;					
								
							else
								o_SEL_ULA  	<= "000";
								o_SEL_IMED 	<= "00";
								o_WR_BCO   	<= '0';
								o_WR_RAM   	<= '0';
								o_WR_IO	   	<= '0';
								o_RD_IO    	<= '0';
								o_PUSH     	<= '0';
								o_POP 	   	<= '0';
								o_RET		<= '0';
								w_STATE 	<= st_FETCH;					
							end if;
						
					when st_EXECUTE =>		
						o_WR_BCO   <= '0';
						o_WR_RAM   <= '0';
						o_WR_IO	   <= '0';
						o_RD_IO    <= '0';
						o_PUSH     <= '0';
						o_POP 	   <= '0';
						w_PC	   <= w_PC + 1;
						o_EN_ROM   <= '1';
						w_STATE    <= st_FETCH;						
					
				    when st_RET => 
						o_POP      <= '0';
						o_WR_BCO   <= '1';
						o_RET	   <= '1';
						w_PC	   <= i_NEXT_PC + 1;					
						o_EN_ROM   <= '1';
						w_STATE    <= st_FETCH;					
											
					when st_CALL => 
						o_PUSH  	<= '0';
						w_PC		<= i_DATA((p_DATA_WIDTH-8) DOWNTO 0);
						o_EN_ROM	<= '1';
						w_STATE 	<= st_FETCH;
					
					
					when st_CALL_INT =>
						o_PUSH		<= '0';
						
						if (i_INT0 = '1') then
							o_INT_ADD <= "01";
						elsif (i_INT1 = '1') then
							o_INT_ADD <= "10";
						else
							o_INT_ADD <= "11";
						end if;
						
						w_PC <= (others => '0');
						o_EN_ROM	<= '1';
						w_STATE 	<= st_FETCH;
								
								
					when st_INPUT =>
						o_WR_BCO 	<= '1';
						o_RD_IO		<= '0';
						w_PC	    <= w_PC + 1;
						o_EN_ROM	<= '1';
						w_STATE  	<= st_FETCH;
					
					when st_JMPI =>
						o_EN_ROM   <= '1';
						w_STATE    <= st_FETCH;
			
					when st_JMPZ => 
						o_EN_ROM   <= '1';
						if (i_DATA_ULA = '1') then
							w_PC <= i_DATA((p_DATA_WIDTH-8) DOWNTO 0);
						else
							w_PC <= w_PC + 1;
						end if;
						w_STATE  <= st_FETCH;
						
					when st_CMP => 
						o_EN_ROM   <= '1';
						if (i_DATA_ULA = '1') then
							v_COMP := 1;
						else 
							v_COMP := 0;
						end if;
						w_PC 	<= w_PC + 1;
						w_STATE <= st_FETCH;

					when st_JMPE => 
						o_EN_ROM   <= '1';
						if (v_COMP = 1) then
							w_PC <= i_DATA((p_DATA_WIDTH-8) DOWNTO 0);
							v_COMP := 0;
						else
							w_PC <= w_PC + 1;
						end if;
						w_STATE  <= st_FETCH;
						
					when st_STOP =>
						w_STATE <= st_STOP;
						
						
					when others => 																
						w_STATE <= st_FETCH;																				
			end case;																				

		end if;																						
	end process U_MACHINE;																	
		
-----------------------------------------------------------------	
end behavioral;